using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickOption : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    
    public GameObject GetPrefab()
    {
        return brickPrefab;
    }
}
