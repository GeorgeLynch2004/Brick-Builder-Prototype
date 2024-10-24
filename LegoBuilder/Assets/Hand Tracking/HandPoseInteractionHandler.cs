using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class HandPoseInteractionHandler : MonoBehaviour
{
    [SerializeField] private List<string> pickupTags;
    [SerializeField] private Transform indexTip;
    [SerializeField] private Transform thumbTip;
    [SerializeField] private bool isPinching;
    [SerializeField] private GameObject objectInCollision;
    [SerializeField] private GameObject objectInHand;

    [SerializeField] private Transform wrist;
    [SerializeField] private Transform angleAssistX;
    [SerializeField] private Transform middleBase;
    [SerializeField] private Transform angleAssistZ;

    [SerializeField] private Vector3 objectRotationZOffset;
    [SerializeField] private Vector3 objectRotationXOffset;
    [SerializeField] private bool releaseImpulse;
    [SerializeField] private Animator handModelAnimator;
    [SerializeField] private Outline outline;
    [SerializeField] private AudioSource selectSound;

    private Quaternion objectInHandRotation;

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
        

        if (objectInHand != null)
        {
            objectInHand.transform.position = transform.position;
            outline.OutlineColor = (Color.green);
        }
        else
        {
            outline.OutlineColor = (Color.white);
        }
        

        isPinching = Vector3.Distance(indexTip.position, thumbTip.position) < .5f;

        if (isPinching)
        {
            if (handModelAnimator != null){handModelAnimator.SetBool("Pinching", true);}
        }
        else
        {
            if (handModelAnimator != null){handModelAnimator.SetBool("Pinching", false);}
        }

        CheckForGrab();

        // Adjust rotation
        float angleZ = UpdateObjectRotation(wrist.position, angleAssistZ.position, middleBase.position);
        float angleX = UpdateObjectRotation(wrist.position, angleAssistX.position, middleBase.position);

        Quaternion rotationZ = Quaternion.Euler(angleZ, 0, 0);
        Quaternion rotationX = Quaternion.Euler(-90f, angleX -90f, 0);

        Quaternion finalRotation = rotationZ * rotationX;

        objectInHandRotation = finalRotation;

        if (objectInHand != null)
        {
            // Ensure object in hand collider is trigger
            objectInHand.GetComponent<Collider>().isTrigger = true;

            objectInHand.transform.rotation = finalRotation;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (!pickupTags.Contains(other.gameObject.tag)) return;

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
            CreateSoundObject(selectSound.clip);

            objectInCollision.transform.SetParent(transform);
            objectInCollision.GetComponent<Collider>().isTrigger = true;
            
            // if there are any brick children in the collision object detach them before picking the object up
            BrickSnapper[] brickChildren = objectInCollision.GetComponentsInChildren<BrickSnapper>();

            if (brickChildren.Length > 0)
            {
                foreach(BrickSnapper child in brickChildren)
                {
                    if (child.gameObject != objectInCollision.gameObject)
                    {
                        child.transform.parent = null;
                    }
                    
                }
            }

            objectInCollision = null;
        }
        // To release an object
        else if (!isPinching && objectInHand != null)
        {
            objectInHand.GetComponent<Collider>().isTrigger = false;
            
            if (objectInHand.GetComponent<BrickSnapper>() != null)
            {
                objectInHand.GetComponent<BrickSnapper>().SnapToGhost();
            }

            transform.DetachChildren();
        }
    }

    public void AssignObjectInHand(GameObject obj)
    {

        objectInHand = Instantiate(obj);
        objectInHand.transform.SetParent(transform);
        objectInHand.GetComponent<Collider>().isTrigger = true;
    }

    private float UpdateObjectRotation(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        Vector3 side1 = point1 - point0;
        Vector3 side2 = point2 - point0;
        return Vector3.Angle(side1, side2);
    }

    public bool IsPinching() {return isPinching;}
    public bool HandsFull() {return objectInHand != null;}

    public Quaternion GetRotation(){return objectInHandRotation;}

    private IEnumerator CreateSoundObject(AudioClip clip)
    {
        GameObject soundObject = new GameObject();
        soundObject.AddComponent<AudioSource>();
        soundObject.GetComponent<AudioSource>().clip = clip;
        soundObject = Instantiate(soundObject);
        soundObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(clip.length);
        Destroy(soundObject);
    }

}
