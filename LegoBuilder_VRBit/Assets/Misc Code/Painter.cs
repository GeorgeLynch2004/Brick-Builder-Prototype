using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Painter : MonoBehaviour
{
    [SerializeField] private Vector3 raycastHitPosition;
    [SerializeField] private Vector3Int currentCellPosition;
    [SerializeField] private GameObjectTile gameObjectTile;
    [SerializeField] private GameObject ghostBlockPrefab;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RaycastHit mostRecentHit;
    [SerializeField] private GameObject ghostBlockInstance;
    [SerializeField] private GestureDetector gestureDetector;
    private bool isPinching;

    private void Start()
    {
        ghostBlockInstance = Instantiate(ghostBlockPrefab);
        ghostBlockInstance.SetActive(false);
    }

    private void Update()
    {

        // Fire a raycast vertically down every frame to check where the hand is hovering above
        ProjectRaycast();
        UpdateGhostBlock();
        
        tilemap = GetNearestTilemap();
        Vector3 position = Vector3Int.RoundToInt(raycastHitPosition);
        currentCellPosition = tilemap.WorldToCell(position);

        // if the pinch was released on the last frame
        if (Input.GetKeyDown(KeyCode.Space));
        {
            PlaceTile();
        }

        isPinching = gestureDetector.IsPinching();
    }

    private void PlaceTile()
    {   
        if (tilemap == null || gameObjectTile == null) return;
        tilemap.SetTile(currentCellPosition, gameObjectTile);
    }

    private void UpdateGhostBlock()
    {
        ghostBlockInstance.transform.position = tilemap.GetCellCenterWorld(currentCellPosition);

        if (!ghostBlockInstance.activeInHierarchy)
        {
            ghostBlockInstance.SetActive(true);
        }
    }


    private Tilemap GetNearestTilemap()
    {
        Tilemap[] tilemapsInScene = GetTilemapsInScene();

        Tilemap closestTilemap = null;
        float closestDistance = Mathf.Infinity;


        Vector3 referencePosition = raycastHitPosition;

        foreach (Tilemap tilemap in tilemapsInScene)
        {
            float distance = Vector3.Distance(tilemap.transform.position, referencePosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTilemap = tilemap;
            }
        }
        return closestTilemap;
    }

    private Tilemap[] GetTilemapsInScene()
    {
        return FindObjectsOfType<Tilemap>();
    }

    private void ProjectRaycast()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out mostRecentHit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, -Vector3.up * mostRecentHit.distance, Color.green);
            
        }
        else
        {
            Debug.DrawRay(transform.position, -Vector3.up * mostRecentHit.distance, Color.red);
        }

        raycastHitPosition = mostRecentHit.point;
    }
}
