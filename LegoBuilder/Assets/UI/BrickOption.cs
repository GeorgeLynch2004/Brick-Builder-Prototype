using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickOption : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private RawImage previewImage;
    
    private void Start()
    {
        previewImage = GetComponent<RawImage>();
        
        Texture2D preview = LoadPrefabPreview(brickPrefab);
        if (preview != null)
        {
            previewImage.texture = preview;
        }
    }

    public GameObject GetPrefab()
    {
        return brickPrefab;
    }

    public Texture2D LoadPrefabPreview(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        string previewPath = $"PrefabPreviews/{prefab.name}_preview";
        Texture2D preview = Resources.Load<Texture2D>(previewPath);

        return preview;
    }
}