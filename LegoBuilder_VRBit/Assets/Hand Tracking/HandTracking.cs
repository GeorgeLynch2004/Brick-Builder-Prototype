using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public GameObject[] handPoints;
    [SerializeField] private GameObject m_HandParent;
    [SerializeField] public Vector3 m_HandPositionOffset;
    [SerializeField] private Vector3 m_HandRotationOffset;
    [SerializeField] private float estimatedDistance;

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
            Debug.Log($"Raw distance value: '{rawDistance}'");

            if (float.TryParse(rawDistance, out float distance))
            {
                estimatedDistance = distance;
                Debug.Log($"Parsed distance: {estimatedDistance} cm");
            }
            else
            {
                Debug.LogWarning($"Failed to parse distance value: '{rawDistance}'");
            }

            m_HandPositionOffset.z = estimatedDistance;


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
                    handPoints[i].transform.localPosition = new Vector3(x, y, z) + m_HandPositionOffset;
                }
                else
                {
                    Debug.LogWarning($"Failed to parse coordinates for point {i}");
                }
            }

            m_HandParent.transform.localEulerAngles = m_HandRotationOffset;
        }
        else
        {
            Debug.LogWarning("Received data does not contain enough points");
        }
    }
}