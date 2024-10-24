using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseplateRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private void Update() {
        
        Vector3 rot = transform.localEulerAngles;

        if (Input.GetKeyDown(KeyCode.A))
        {
            rot.y += 90f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rot.y -= 90f;
        }

        transform.localEulerAngles = rot;
    }
}
