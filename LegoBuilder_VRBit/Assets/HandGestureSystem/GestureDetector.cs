using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Gesture
{
    private string name;
    public Dictionary<Transform[], CriteriaContent> criteria;

    public Gesture(string name)
    {
        this.name = name;
        this.criteria = new Dictionary<Transform[], CriteriaContent>();
    }

    public void AddCriteria(Transform t1, Transform t2, float distance, bool greaterFlag)
    {
        Transform[] transforms = new Transform[] { t1, t2 };
        CriteriaContent criteriaContent = new CriteriaContent(distance, greaterFlag);
        criteria.Add(transforms, criteriaContent);
    }
}

public struct CriteriaContent
{
    public float distance;
    public bool greaterFlag; // true to check for distance greater than, false to check for distance lesser than

    public CriteriaContent(float distance, bool greaterFlag)
    {
        this.distance = distance;
        this.greaterFlag = greaterFlag;
    }
}

public class GestureDetector : MonoBehaviour
{
    private Dictionary<string, Gesture> m_Gestures;
    [SerializeField] private Transform m_Thumb;
    [SerializeField] private Transform m_Index;
    [SerializeField] private Transform m_Middle;
    [SerializeField] private Transform m_Ring;
    [SerializeField] private Transform m_Pinky;
    [SerializeField] private bool m_Pinching;

    private void Start() 
    {
        InitialiseGestures();
    }

    private void Update() 
    {
        if (m_Gestures.TryGetValue("Pinching", out Gesture pinchingGesture))
        {
            m_Pinching = CheckForGesture(pinchingGesture);
        } 
    }

    private void InitialiseGestures()
    {
        m_Gestures = new Dictionary<string, Gesture>();

        Gesture pinching = new Gesture("Pinching");  
        pinching.AddCriteria(m_Thumb, m_Index, .5f, false);
        pinching.AddCriteria(m_Index, m_Middle, .7f, true);
        pinching.AddCriteria(m_Index, m_Middle, 1f, true);
        pinching.AddCriteria(m_Index, m_Ring, 1.3f, true);
        pinching.AddCriteria(m_Index, m_Pinky, 1.6f, true);
        m_Gestures.Add("Pinching", pinching);
    }

    private bool CheckForGesture(Gesture gesture)
    {
        foreach (var entry in gesture.criteria)
        {
            Transform[] transforms = entry.Key;
            CriteriaContent criteriaContent = entry.Value;
            float criteriaDistance = criteriaContent.distance;
            bool greaterFlag = criteriaContent.greaterFlag;

            if (transforms.Length != 2)
            {
                Debug.LogWarning("Transforms for the gesture criteria aren't as a pair!");
                continue;
            }

            Transform t1 = transforms[0];
            Transform t2 = transforms[1];

            float distance = Vector3.Distance(t1.position, t2.position);

            if (greaterFlag)
            {
                if (distance < criteriaDistance)
                {
                    return false;
                }
            }
            else
            {
                if (distance > criteriaDistance)
                {
                    return false;
                }
            }
            
        }
        return true;
    }

    public bool IsPinching()
    {
        return m_Pinching;
    }
}
