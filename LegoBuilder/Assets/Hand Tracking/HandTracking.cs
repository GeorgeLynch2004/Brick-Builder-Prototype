using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] handPoints;
    [SerializeField] private GameObject m_HandParent;
    [SerializeField] public Vector3 m_HandPositionOffset;
    [SerializeField] private Vector3 m_HandRotationOffset;
    [SerializeField] private float estimatedDistance;
    [SerializeField] private float standardHandSize; // Adjust this value as needed
    [SerializeField] private Vector3[] normalizedLandmarks;

    void Update()
    {
        HandPositionalDataProcessing();
    }

    private void HandPositionalDataProcessing()
    {
        string data = udpReceive.data;

        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);
        string[] points = data.Split(',');

        if (points.Length >= 64)
        {
            // Parse the distance (last element)
            string rawDistance = points[points.Length - 1].Trim();

            if (float.TryParse(rawDistance, out float distance))
            {
                estimatedDistance = distance;
            }
            else
            {
                Debug.LogWarning($"Failed to parse distance value: '{rawDistance}'");
            }

            m_HandPositionOffset.z = estimatedDistance;

            Vector3[] rawLandmarks = new Vector3[21];

            // Parse all the lm points
            for (int i = 0; i < 21; i++)
            {
                if (float.TryParse(points[i * 3], out float x) &&
                    float.TryParse(points[i * 3 + 1], out float y) &&
                    float.TryParse(points[i * 3 + 2], out float z))
                {
                    x = 7 - x / 100;
                    y = y / 100;
                    z = z / 100;
                    rawLandmarks[i] = new Vector3(x, y, z);
                }
                else
                {
                    Debug.LogWarning($"Failed to parse coordinates for point {i}");
                }
            }

            normalizedLandmarks = NormalizeHandSize(rawLandmarks);

            for (int i = 0; i < 21; i++)
            {
                normalizedLandmarks[i] += m_HandPositionOffset;
                handPoints[i].transform.localPosition = normalizedLandmarks[i];
            }

            m_HandParent.transform.localEulerAngles = m_HandRotationOffset;
        }
        else
        {
            Debug.LogWarning("Received data does not contain enough points");
        }
    }

    private Vector3[] NormalizeHandSize(Vector3[] landmarks)
    {
        Vector3 wristPosition = landmarks[5];
        Vector3 middleFingerTipPosition = landmarks[17]; // Assuming index 12 is the middle fingertip

        float currentSize = Vector3.Distance(wristPosition, middleFingerTipPosition);
        float scaleFactor = standardHandSize / currentSize;

        Vector3[] normalizedLandmarks = new Vector3[landmarks.Length];

        for (int i = 0; i < landmarks.Length; i++)
        {
            Vector3 relativePosition = landmarks[i] - wristPosition;
            normalizedLandmarks[i] = wristPosition + (relativePosition * scaleFactor);
        }

        return normalizedLandmarks;
    }

    public Vector3[] GetNormalizedLandmarks()
    {
        return normalizedLandmarks;
    }
}