using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BrickSnapper : MonoBehaviour
{
    [SerializeField] private HandPoseInteractionHandler handPoseInteractionHandler;
    [SerializeField] private Transform snapPointParent;
    [SerializeField] private List<Transform> snapPoints;
    [SerializeField] private float snapRange = 0.5f;
    [SerializeField] private GameObject ghostBrickPrefab;
    [SerializeField] private bool debugMode;
    [SerializeField] private GameObject ghostBrickInstance;
    [SerializeField] private BrickSnapper currentTargetBrick;
    public bool brickInHand;

    private void Start() {

        snapPoints = new List<Transform>();

        foreach (Transform child in snapPointParent)
        {
            if (child.gameObject.tag == "Snap Point")
            {
                snapPoints.Add(child);
            }
        }
    }

    private void Update()
    {

        // We only want the brick in the hand of the player to handle the snapping mechanism
        if (transform.parent.CompareTag("Interaction Handler"))
        {
            brickInHand = true;
            handPoseInteractionHandler = transform.parent.GetComponent<HandPoseInteractionHandler>();
            HandleSnapping();
        }
        else
        {
            brickInHand = false;
            handPoseInteractionHandler = null;
        }

        // Debug visualization
        if (debugMode)
        {
            VisualizeSnapPoints();
        }
    }

    private void HandleSnapping()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, snapRange);
        BrickSnapper closestBrick = null;
        float closestDistance = float.MaxValue;
        BrickSnapper[] currentBrickChildren = transform.GetComponentsInChildren<BrickSnapper>();

        foreach (Collider col in nearbyColliders)
        {
            if (col.gameObject != gameObject && (col.CompareTag("Brick") || col.CompareTag("Baseplate")))
            {
                BrickSnapper otherBrick = col.GetComponent<BrickSnapper>();
                if (otherBrick != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);

                    // check if there is a brick as parent and ensure that that cannot be set as the closestbrick as we want bricks in hand to be detected

                    if (distance < closestDistance && !currentBrickChildren.Contains(otherBrick))
                    {
                        closestDistance = distance;
                        closestBrick = otherBrick;
                    }
                }
            }
        }

        if (closestBrick != null)
        {
            SnapData snapData = FindBestSnapPoints(snapPoints, closestBrick.snapPoints);
            if (snapData.isValid)
            {
                UpdateGhostBrick(snapData, closestBrick);
                currentTargetBrick = closestBrick;
            }
            else
            {
                currentTargetBrick = null;
            }
        }
        else
        {
            currentTargetBrick = null;
            DestroyGhostBrick();
        }
    }

    private SnapData FindBestSnapPoints(List<Transform> originPoints, List<Transform> targetPoints)
    {
        SnapData bestSnap = new SnapData();
        float closestDistance = float.MaxValue;

        foreach (Transform originPoint in originPoints)
        {
            foreach (Transform targetPoint in targetPoints)
            {
                float distance = Vector3.Distance(originPoint.position, targetPoint.position);
                if (distance < closestDistance && distance <= snapRange)
                {
                    closestDistance = distance;
                    bestSnap.originPoint = originPoint;
                    bestSnap.targetPoint = targetPoint;
                    bestSnap.isValid = true;
                }
            }
        }

        return bestSnap;
    }

    private void UpdateGhostBrick(SnapData snapData, BrickSnapper targetBrick)
    {
        if (ghostBrickInstance == null)
        {
            ghostBrickInstance = Instantiate(ghostBrickPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Ghost brick instantiated");
        }

        // Calculate the position offset
        Vector3 positionOffset = transform.position - snapData.originPoint.position;

        // Set the ghost brick position
        Vector3 snappedPosition = targetBrick.transform.TransformPoint(targetBrick.transform.InverseTransformPoint(snapData.targetPoint.position) + positionOffset);
        ghostBrickInstance.transform.position = RoundToNearestWhole(snappedPosition);

        // Calculate and apply the rotation
        Quaternion targetRotation = targetBrick.transform.rotation * Quaternion.Inverse(snapData.targetPoint.rotation) * snapData.originPoint.rotation;
        Vector3 roundedEuler = RoundToNearestRightAngle(targetRotation.eulerAngles);
        ghostBrickInstance.transform.rotation = Quaternion.Euler(roundedEuler);

        if (debugMode)
        {
            Debug.DrawLine(snapData.originPoint.position, snapData.targetPoint.position, Color.yellow);
            Debug.Log($"Ghost brick position: {ghostBrickInstance.transform.position}");
            Debug.Log($"Ghost brick rotation: {ghostBrickInstance.transform.rotation.eulerAngles}");
        }
    }

    private Vector3 RoundToNearestRightAngle(Vector3 eulerAngles)
    {
        return new Vector3(
            Mathf.Round(eulerAngles.x / 90f) * 90f,
            Mathf.Round(eulerAngles.y / 90f) * 90f,
            Mathf.Round(eulerAngles.z / 90f) * 90f
        );
    }

    private Vector3 RoundToNearestWhole(Vector3 v)
    {
        return new Vector3(
            Round(v.x),
            Round(v.y),  
            Round(v.z)
        );
    }

    public void SnapToGhost()
    {
        Debug.Log("SnapToGhost called");
        if (ghostBrickInstance != null && currentTargetBrick != null)
        {
            if (debugMode)
            {
                Debug.Log($"Ghost position before snap: {ghostBrickInstance.transform.position}");
                Debug.Log($"Ghost rotation before snap: {ghostBrickInstance.transform.rotation.eulerAngles}");
                Debug.Log($"Current brick position before snap: {transform.position}");
                Debug.Log($"Current brick rotation before snap: {transform.rotation.eulerAngles}");
            }
            

            // Find the best matching snap points
            SnapData snapData = FindBestSnapPoints(snapPoints, currentTargetBrick.snapPoints);

            if (snapData.isValid && ghostBrickInstance.GetComponent<GhostBrick>().GetPositionSnappable())
            {

                // Set the brick's position and rotation (delayed so that the hand rotation code doesn't override the brick snapping code)
                StartCoroutine(DelayedUpdateObjectTransform(transform, ghostBrickInstance.transform.position, ghostBrickInstance.transform.localRotation));

                // Set the parent after updating position and rotation
                transform.SetParent(currentTargetBrick.transform);
                
                // Make the Rigidbody kinematic to prevent further physics interactions
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    Debug.Log("Rigidbody set to kinematic");
                }
                else
                {
                    Debug.LogWarning("No Rigidbody component found on the brick");
                }
                
                if (debugMode)
                {
                    Debug.Log($"Brick position after snap: {transform.position}");
                    Debug.Log($"Brick rotation after snap: {transform.rotation.eulerAngles}");
                    Debug.Log($"Snapped to {currentTargetBrick.name}");
                }
                

                // Destroy the ghost brick after snapping
                DestroyGhostBrick();
            }
            else
            {
                Debug.LogWarning("Failed to find valid snap points");
                DestroyGhostBrick();
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Failed to snap: Ghost brick or target brick is null");
        }
    }

    private IEnumerator DelayedUpdateObjectTransform(Transform transform, Vector3 position, Quaternion rotation)
    {
        yield return null;
        transform.position = position;
        transform.localRotation = rotation;
    }

    private void DestroyGhostBrick()
    {
        if (ghostBrickInstance != null)
        {
            Debug.Log("Destroying ghost brick");
            Destroy(ghostBrickInstance);
            ghostBrickInstance = null;
        }
    }

    private void VisualizeSnapPoints()
    {
        foreach (Transform point in snapPoints)
        {
            Debug.DrawRay(point.position, point.forward * 0.1f, Color.blue);
        }
    }

    private float Round(float value)
    {
        return Mathf.Round(value * 2) / 2;
    }


    private struct SnapData
    {
        public Transform originPoint;
        public Transform targetPoint;
        public bool isValid;
    }
}