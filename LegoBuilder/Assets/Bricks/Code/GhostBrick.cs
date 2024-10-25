using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBrick : MonoBehaviour
{
    [SerializeField] private bool positionSnappable; // Boolean to represent whether the current position of the ghost brick is available for the brick in hand to snap to
    [SerializeField] private GameObject collision;
    [SerializeField] private Material blue;
    [SerializeField] private Material red;
    [SerializeField] private GameObject model;

    private void Start() {
        if (collision == null)
        {
            positionSnappable = true;
        }
        else
        {
            positionSnappable = false;
        }
    }

    private void Update()
    {
        if (positionSnappable)
        {
            model.GetComponent<MeshRenderer>().material = blue;
        }
        else
        {
            model.GetComponent<MeshRenderer>().material = red;
        }
    }

    public bool GetPositionSnappable()
    {
        return positionSnappable;
    }

    private void OnTriggerEnter(Collider other) {
        if ((other.gameObject.CompareTag("Brick") || other.gameObject.CompareTag("Baseplate")) && collision == null && !other.gameObject.GetComponent<BrickSnapper>().brickInHand)
        {
            collision = other.gameObject;
            positionSnappable = false;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == collision)
        {
            collision = null;
            positionSnappable = true;
        }
    }
}
