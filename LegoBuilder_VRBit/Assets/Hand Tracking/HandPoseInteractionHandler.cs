using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HandPoseInteractionHandler : MonoBehaviour
{
    [SerializeField] private Transform indexTip;
    [SerializeField] private Transform thumbTip;
    [SerializeField] private bool isPinching;
    [SerializeField] private List<GameObject> objectsInCollision;
    [SerializeField] private GameObject objectInHand;

    private void Update() {

        transform.position = 0.5f*(indexTip.position + thumbTip.position);

        if (objectInHand != null){objectInHand.transform.position = transform.position;}
        

        isPinching = Vector3.Distance(indexTip.position, thumbTip.position) < 1f;

        CheckForGrab();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (!objectsInCollision.Contains(other.gameObject))
        {
            objectsInCollision.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (objectsInCollision.Contains(other.gameObject))
        {
            objectsInCollision.Remove(other.gameObject);
        }
    }

    private void CheckForGrab()
    {
        if (isPinching && objectsInCollision.Count > 0)
        {
            objectInHand = objectsInCollision[0];
            objectInHand.transform.SetParent(transform);
            objectsInCollision.RemoveAt(0);

            Rigidbody objectRigidbody = objectInHand.GetComponent<Rigidbody>();
            if (objectRigidbody != null)
            {
                objectRigidbody.isKinematic = true;
            }
        }

        if (!isPinching && objectInHand != null)
        {
            objectInHand.transform.SetParent(null);
            objectsInCollision.Add(objectInHand);

            Rigidbody objectRigidbody = objectInHand.GetComponent<Rigidbody>();
            if (objectRigidbody != null)
            {
                objectRigidbody.isKinematic = false;
            }

            objectInHand = null;
        }
    }
}
