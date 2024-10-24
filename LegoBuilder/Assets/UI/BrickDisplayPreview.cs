using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BrickDisplayPreview : MonoBehaviour
{
    [SerializeField] private UIPointer uiPointer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI brickNameText;
    [SerializeField] private Vector3 scaleAdjustment;
    private GameObject previousFrameCollision;
    private GameObject currentDisplayedObject;

    private void Update()
    {
        RotateObject();

        GameObject currentCollision = uiPointer.GetCurrentCollision();

        // Check if the collision has changed
        if (currentCollision != previousFrameCollision)
        {
            // Destroy the currently displayed object, if any
            if (currentDisplayedObject != null)
            {
                Destroy(currentDisplayedObject);
                currentDisplayedObject = null;
            }

            // If there's a new collision, instantiate the new object
            if (currentCollision != null)
            {
                BrickOption brickOption = currentCollision.GetComponent<BrickOption>();
                if (brickOption != null)
                {
                    GameObject objectToDisplay = brickOption.GetPrefab();
                    if (objectToDisplay != null)
                    {
                        currentDisplayedObject = Instantiate(objectToDisplay, spawnPoint.transform);
                        currentDisplayedObject.transform.localScale = scaleAdjustment;
                        currentDisplayedObject.transform.localPosition = Vector3.zero;
                        currentDisplayedObject.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            previousFrameCollision = currentCollision;
        }

        if (currentDisplayedObject != null)
        {
            brickNameText.text = currentDisplayedObject.name.Replace("(Clone)", "");
        }
        else
        {
            brickNameText.text = "";
        }
    }

    private void RotateObject()
    {
        Vector3 rot = transform.localEulerAngles;

        rot.y += 50f * Time.deltaTime;

        transform.localEulerAngles = rot;
    }
}