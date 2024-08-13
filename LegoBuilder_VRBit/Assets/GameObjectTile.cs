using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New GameObject Tile", menuName = "Tiles/GameObject Tile")]
public class GameObjectTile : TileBase
{
    [SerializeField] public GameObject prefab;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // Set the GameObject for this tile
        tileData.gameObject = prefab;

        // Adjust the transform matrix if needed
        Matrix4x4 transformMatrix = Matrix4x4.identity;

        // You can adjust the position here if the prefab needs to be offset
        transformMatrix.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
        
        tileData.transform = transformMatrix;
        tileData.flags = TileFlags.LockTransform;
        tileData.colliderType = Tile.ColliderType.None;
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (prefab != null)
        {
            // Get the Tilemap component from the ITilemap
            Tilemap tm = tilemap.GetComponent<Tilemap>();
            if (tm != null)
            {
                Vector3 worldPosition = tm.CellToWorld(position);
                
            }
        }
        return base.StartUp(position, tilemap, go);
    }
}

