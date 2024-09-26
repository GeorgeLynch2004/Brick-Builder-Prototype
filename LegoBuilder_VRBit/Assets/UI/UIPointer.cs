using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPointer : MonoBehaviour
{
    [SerializeField, Tooltip("This object transform will be used to control the position of the pointer.")]
    private Transform interactionIndicatorObject;
    [SerializeField, Tooltip("Experimental multiplier value to ensure that the full range of the canvas is reachable.")]
    private float movementMultiplier;
    [SerializeField, Tooltip("Adjustable offset vector to apply to the position to ensure that the full range of the canvas is reachable.")]
    private Vector2 offsetVector;
    [SerializeField, Tooltip("Reference to HandPoseInteractionHandler on the hand model.")]
    private HandPoseInteractionHandler handPoseInteractionHandler;
    [SerializeField, Tooltip("Current Collision")]
    private GameObject currentCollision;

    [SerializeField] private Sprite[] cursorImages;
    [SerializeField] private Image cursorObject;

    private void Update()
    {
        UpdateTransform();

        if (handPoseInteractionHandler != null)
        {
            if (handPoseInteractionHandler.IsPinching())
            {
                GrabOption();
            }
            else
            {
                ChangeVisuals(Color.red, cursorImages[0]);
            }
        }
    }

    // Called every frame to update the position of the pointer based on the indicators position
    private void UpdateTransform()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(interactionIndicatorObject.position.x * movementMultiplier, interactionIndicatorObject.position.y * movementMultiplier) + offsetVector;
    }

    private void GrabOption()
    {
        // change colour to green
        ChangeVisuals(Color.blue, cursorImages[1]);

        // check for if there is a collision with a brick option UI
        if (currentCollision.GetComponent<BrickOption>() != null && !handPoseInteractionHandler.HandsFull())
        {
            handPoseInteractionHandler.AssignObjectInHand(currentCollision.GetComponent<BrickOption>().GetPrefab());
        }

        // check for if there is a collision with a baseplate rotation button
        if (currentCollision.GetComponent<BaseplateRotationButton>() != null && !handPoseInteractionHandler.HandsFull())
        {
            BaseplateRotationButton baseplateRotation = currentCollision.GetComponent<BaseplateRotationButton>();
            baseplateRotation.RotateBaseplate(baseplateRotation.direction);
        }

    }

    private void ChangeVisuals(Color color, Sprite cursor)
    {
        Image image = GetComponent<Image>();
        image.color = color;
        cursorObject.sprite = cursor;
    }

    public GameObject GetCurrentCollision()
    {
        return currentCollision;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (currentCollision == null)
        {
            currentCollision = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        currentCollision = null;
    }
}
