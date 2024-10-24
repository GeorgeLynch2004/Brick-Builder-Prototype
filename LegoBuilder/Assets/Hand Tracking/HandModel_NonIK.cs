using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandModel_NonIK : MonoBehaviour
{
    [SerializeField] private HandPoseInteractionHandler handPoseInteractionHandler;
    [SerializeField] private HandTracking handTrackingComponent;
    [SerializeField] private Transform landmarkWrist;
    [SerializeField] private Transform modelWrist;
    [SerializeField] private Vector3 wristRotationOffset;
    private Vector3[] landmarks;

    private void Update() 
    {
        landmarks = handTrackingComponent.GetNormalizedLandmarks();

        modelWrist.position = landmarkWrist.position;
        modelWrist.rotation = CalculateWristRotation();
    }

    Quaternion CalculateWristRotation()
    {
        Vector3 wristToIndex = landmarks[5] - landmarks[0];
        Vector3 wristToPinky = landmarks[17] - landmarks[0];
        Vector3 palmNormal = Vector3.Cross(wristToIndex, wristToPinky).normalized;

        Quaternion baseRotation = Quaternion.LookRotation(wristToIndex, palmNormal);
        
        // Apply the offset rotation
        Quaternion offsetRotation = Quaternion.Euler(wristRotationOffset);
        return baseRotation * offsetRotation;
    }
}
