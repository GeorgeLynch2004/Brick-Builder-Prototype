using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class HandPoseInteractionHandler : MonoBehaviour
{
    [SerializeField] private Transform indexTip;
    [SerializeField] private Transform thumbTip;
    [SerializeField] private bool isPinching;
    [SerializeField] private GameObject objectInCollision;
    [SerializeField] private GameObject objectInHand;

    private void Update() {

        transform.position = 0.5f*(indexTip.position + thumbTip.position);

        try
        {
            objectInHand = transform.GetChild(0).gameObject;
        }
        catch (Exception e)
        {
            objectInHand = null;
        }
        

        if (objectInHand != null){objectInHand.transform.position = transform.position;}
        

        isPinching = Vector3.Distance(indexTip.position, thumbTip.position) < 1f;

        CheckForGrab();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (objectInCollision == null)
        {
            objectInCollision = other.gameObject;
        }
        
    }

    private void OnTriggerExit(Collider other) 
    {
        objectInCollision = null;
    }

    private void CheckForGrab()
    {
        // To pick up an object
        if (isPinching && objectInCollision != null && objectInHand == null)
        {
            objectInCollision.transform.SetParent(transform);
            objectInCollision = null;
        }
        // To release an object
        else if (!isPinching && objectInHand != null)
        {
            transform.DetachChildren();
        }
    }

    public void AssignObjectInHand(GameObject obj)
    {

        objectInHand = Instantiate(obj);
        objectInHand.transform.SetParent(transform);
    }

    public bool IsPinching() {return isPinching;}
    public bool HandsFull() {return objectInHand != null;}
}
