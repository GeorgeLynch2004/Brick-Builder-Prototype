using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseplateRotationButton : MonoBehaviour
{
    public enum RotationDirection
    {
        Left,
        Right
    }

    [SerializeField] public RotationDirection direction;
    [SerializeField] private Transform baseplate;
    private bool buttonPressCooldown;


    public void RotateBaseplate(RotationDirection direction)
    {
        if (buttonPressCooldown) return;

        Vector3 rot = baseplate.localEulerAngles;

        if (direction == RotationDirection.Left)
        {
            rot.y += 90f;
        }
        if (direction == RotationDirection.Right)
        {
            rot.y -= 90f;
        }

        baseplate.localEulerAngles = rot;

        StartCoroutine(RunCountdown());
    }

    private IEnumerator RunCountdown()
    {
        buttonPressCooldown = true;
        yield return new WaitForSeconds(.5f);
        buttonPressCooldown = false;
    }

}
