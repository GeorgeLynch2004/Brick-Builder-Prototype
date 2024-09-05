using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryObjectMovement : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed;

    private void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.W))
        {
            pos += transform.forward * m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            pos -= transform.forward * m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos += transform.right * m_movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            pos -= transform.right * m_movementSpeed * Time.deltaTime;
        }

        transform.position = pos;
    }
}
