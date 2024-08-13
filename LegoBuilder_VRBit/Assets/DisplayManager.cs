using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Displays connected: " + Display.displays.Length);

        // check if a second display is connected
        if (Display.displays.Length <= 1)
        {
            Debug.LogError("ERROR: there is only one display connected");
        }
        else
        {
            Display.displays[1].Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
