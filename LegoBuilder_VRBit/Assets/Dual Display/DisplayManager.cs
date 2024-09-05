using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    [SerializeField] private GameObject handIndicatorObject;
    [SerializeField] private GameObject handModel;
    [SerializeField] private GameObject PointerUI;
    [SerializeField, Tooltip("Value to determine where the scene and the ui should start and stop in world space. E.g. If we want the left hand side of the scene to be the scene view and the right hand side to be UI then we leave the value at 0.")] 
    private float screenDividerX;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

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
        if (handIndicatorObject.transform.position.x < screenDividerX)
        {
            ToggleGameObject(PointerUI, false);
            ToggleGameObject(handModel, true);
        }
        else
        {
            ToggleGameObject(PointerUI, true);
            ToggleGameObject(handModel, false);
        }
    }

    private void ToggleGameObject(GameObject obj, bool toggle)
    {
        obj.SetActive(toggle);
    }
}
