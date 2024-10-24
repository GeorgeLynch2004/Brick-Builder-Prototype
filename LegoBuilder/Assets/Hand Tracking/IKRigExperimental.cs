using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKRigExperimental : MonoBehaviour
{
    [SerializeField] HandPoseInteractionHandler handPoseInteractionHandler;
    [SerializeField] HandTracking handTracking;
    private Vector3[] landmarks;
    [SerializeField] private Transform wrist;
    [SerializeField] private Transform modelWrist;
    [SerializeField] private Vector3 wristRotationOffset;
    [SerializeField] private Transform thumb;
    [SerializeField] private Transform index;
    [SerializeField] private Transform middle;
    [SerializeField] private Transform ring;
    [SerializeField] private Transform pinky;

    [SerializeField] private Transform thumbTarget;
    [SerializeField] private Transform indexTarget;
    [SerializeField] private Transform middleTarget;
    [SerializeField] private Transform ringTarget;
    [SerializeField] private Transform pinkyTarget;

    private void Update()
    {
        landmarks = handTracking.GetNormalizedLandmarks();

        modelWrist.position = wrist.position;
        modelWrist.rotation = CalculateWristRotation();

        thumbTarget.position = thumb.position;
        indexTarget.position = index.position;
        middleTarget.position = middle.position;
        ringTarget.position = ring.position;
        pinkyTarget.position = pinky.position;
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