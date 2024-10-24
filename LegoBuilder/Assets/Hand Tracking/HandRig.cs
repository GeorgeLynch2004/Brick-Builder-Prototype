using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRig : MonoBehaviour
{
    [SerializeField] private HandPoseInteractionHandler handPoseInteractionHandler;
    [SerializeField] private HandTracking handTracking;
    [SerializeField] private float scaleFactor = 0.1f; // Adjust this to scale the hand model
    [SerializeField] private Vector3 offsetPosition = Vector3.zero; // Adjust to reposition the entire hand

    private Vector3[] landmarks;

    [System.Serializable]
    public class FingerBones
    {
        public Transform target;
        public Transform hint;
        public Transform tip;
        public Transform upper;
        public Transform middle;
        public Transform lower;
    }

    [SerializeField] private Transform wrist;
    [SerializeField] private Vector3 wristRotationOffset;
    [SerializeField] private FingerBones thumb;
    [SerializeField] private FingerBones index;
    [SerializeField] private FingerBones middle;
    [SerializeField] private FingerBones ring;
    [SerializeField] private FingerBones pinky;

    private void Update()
    {
        landmarks = handTracking.GetNormalizedLandmarks();
        if (landmarks == null || landmarks.Length != 21) return;

        UpdateWrist();
        UpdateFinger(thumb, 1, 2, 3, 4);
        UpdateFinger(index, 5, 6, 7, 8);
        UpdateFinger(middle, 9, 10, 11, 12);
        UpdateFinger(ring, 13, 14, 15, 16);
        UpdateFinger(pinky, 17, 18, 19, 20);
    }

    private void UpdateWrist()
    {
        wrist.position = landmarks[0];
        wrist.rotation = CalculateWristRotation();
    }

    private void UpdateFinger(FingerBones finger, int baseIndex, int middleIndex, int upperIndex, int tipIndex)
{
    // Set positions for all bones
    finger.lower.position = TransformLandmarkPosition(landmarks[baseIndex]);
    finger.middle.position = TransformLandmarkPosition(landmarks[middleIndex]);
    finger.upper.position = TransformLandmarkPosition(landmarks[upperIndex]);
    finger.tip.position = TransformLandmarkPosition(landmarks[tipIndex]);

    // Set IK target
    finger.target.position = finger.tip.position;

    // Set hint position
    finger.hint.position = finger.middle.position;

    // Update rotations
    finger.lower.rotation = CalculateRotation(landmarks[baseIndex], landmarks[middleIndex]);
    finger.middle.rotation = CalculateRotation(landmarks[middleIndex], landmarks[upperIndex]);
    finger.upper.rotation = CalculateRotation(landmarks[upperIndex], landmarks[tipIndex]);
}

    private Vector3 TransformLandmarkPosition(Vector3 landmark)
    {
        // Convert from MediaPipe coordinate system to Unity
        Vector3 unityLandmark = new Vector3(landmark.x, landmark.y, landmark.z);
        
        // Apply scale and offset
        return (unityLandmark * scaleFactor) + offsetPosition;
    }

    private Quaternion CalculateRotation(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        return Quaternion.LookRotation(direction, Vector3.up);
    }

    private Quaternion CalculateWristRotation()
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