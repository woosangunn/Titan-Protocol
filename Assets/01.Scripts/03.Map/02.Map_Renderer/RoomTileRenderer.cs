using UnityEngine;
using UnityEngine.Tilemaps;
using MyGame.Map;
using System.Collections.Generic;

public class RoomTileRenderer : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;
    public GameObject doorPrefab;

    private List<GameObject> spawnedDoors = new();

    public void ClearTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        foreach (var door in spawnedDoors)
            Destroy(door);
        spawnedDoors.Clear();
    }

    public void DrawRoom(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);
                floorTilemap.SetTile(tilePos, floorTile);

                if (x == 0 || x == room.tileSize.x - 1 || y == 0 || y == room.tileSize.y - 1)
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }

        foreach (Vector2Int localPos in room.doorLocalPositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;
            if (doorTile != null) wallTilemap.SetTile(tilePos, doorTile);

            Vector3 worldPos = wallTilemap.CellToWorld(tilePos) + new Vector3(0f, 0f);
            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);
            DoorTrigger trigger = door.GetComponent<DoorTrigger>();
            trigger.direction = GetDirectionFromDoor(localPos, room.tileSize);
            trigger.roomLoader = FindAnyObjectByType<RoomLoader>();
            spawnedDoors.Add(door);
        }
    }

    private Vector2Int GetDirectionFromDoor(Vector2Int localPos, Vector2Int tileSize)
    {
        if (localPos.x == 0) return Vector2Int.left;
        if (localPos.x == tileSize.x - 1) return Vector2Int.right;
        if (localPos.y == 0) return Vector2Int.down;
        if (localPos.y == tileSize.y - 1) return Vector2Int.up;
        return Vector2Int.zero;
    }

    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
            door.GetComponent<DoorTrigger>()?.Open();
    }
}
