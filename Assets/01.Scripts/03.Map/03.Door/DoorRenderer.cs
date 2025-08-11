using MyGame.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorRenderer : MonoBehaviour
{
    public Tilemap wallTilemap; 
    public TileBase doorTile;
    public GameObject doorPrefab;

    private List<GameObject> spawnedDoors = new();

    public void SpawnDoors(RoomData room, Vector3Int origin, RoomLoader loader)
    {
        ClearDoors();

        foreach (var doorData in room.doors)
        {
            Vector3Int tilePos = origin + (Vector3Int)doorData.localPosition;

            if (doorTile != null)
                wallTilemap.SetTile(tilePos, doorTile);

            Vector3 worldPos = wallTilemap.CellToWorld(tilePos);
            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);

            if (door.TryGetComponent(out DoorTrigger trigger))
            {
                trigger.direction = doorData.direction;

                if (room.type != RoomType.Combat)
                    trigger.Open();
            }

            spawnedDoors.Add(door);
        }
    }

    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
        {
            if (door.TryGetComponent(out DoorTrigger trigger))
                trigger.Open();
        }
    }

    public void ClearDoors()
    {
        foreach (var door in spawnedDoors)
            if (door) Destroy(door);

        spawnedDoors.Clear();
    }
}
