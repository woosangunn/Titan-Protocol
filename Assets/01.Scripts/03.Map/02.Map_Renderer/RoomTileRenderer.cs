using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using MyGame.Map;

public class RoomTileRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [Header("Tiles")]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorClosedTile;
    public TileBase doorOpenTile;

    [Header("Prefabs")]
    public GameObject doorTriggerPrefab;

    private List<GameObject> spawnedTriggers = new();

    public void ClearTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        foreach (var obj in spawnedTriggers)
            Destroy(obj);
        spawnedTriggers.Clear();
    }

    public void DrawRoom(RoomData room, Vector3Int origin)
    {
        Vector2Int size = room.size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = new(x, y, 0);
                floorTilemap.SetTile(pos + origin, floorTile);

                bool isEdge = x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
                if (isEdge)
                    wallTilemap.SetTile(pos + origin, wallTile);
            }
        }

        SpawnDoors(room, origin);
    }

    private void SpawnDoors(RoomData room, Vector3Int origin)
    {
        if (room.doorLocalPositions.Count == 0)
        {
            Debug.LogWarning($"[RoomTileRenderer] {room.position} 문 정보 없음!");
            return;
        }

        for (int i = 0; i < room.doorLocalPositions.Count; i++)
        {
            Vector2Int localPos = room.doorLocalPositions[i];
            Vector3Int tilePos = new(localPos.x, localPos.y, 0);
            Vector3Int worldTilePos = tilePos + origin;

            wallTilemap.SetTile(worldTilePos, doorClosedTile);

            if (doorTriggerPrefab != null)
            {
                Vector3 worldPos = wallTilemap.CellToWorld(worldTilePos) + new Vector3(0.5f, 0.5f);
                GameObject trigger = Instantiate(doorTriggerPrefab, worldPos, Quaternion.identity);
                trigger.SetActive(false);

                DoorTrigger doorTrigger = trigger.GetComponent<DoorTrigger>();

                // room.neighborDirs와 doorLocalPositions의 순서가 일치한다고 가정하고 direction 세팅
                doorTrigger.direction = room.neighborDirs[i];

                spawnedTriggers.Add(trigger);
            }
            else
            {
                Debug.LogWarning("doorTriggerPrefab이 비어 있습니다.");
            }
        }
    }


    public void OpenAllDoors()
    {
        foreach (var obj in spawnedTriggers)
        {
            if (obj == null) continue;

            obj.GetComponent<DoorTrigger>()?.Open();

            Vector3Int cell = wallTilemap.WorldToCell(obj.transform.position);
            wallTilemap.SetTile(cell, doorOpenTile);
        }
    }
}
