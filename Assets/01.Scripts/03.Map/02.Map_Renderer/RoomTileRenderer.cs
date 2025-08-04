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
    public TileBase doorTile;

    [Header("Prefabs")]
    public GameObject doorPrefab;

    private readonly List<GameObject> spawnedDoors = new();

    // �� Ÿ�� ��� ���� �� ������ �� ����
    public void ClearTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        foreach (var door in spawnedDoors)
        {
            if (door != null)
                Destroy(door);
        }
        spawnedDoors.Clear();
    }

    // �� �׸��� �Լ�
    public void DrawRoom(RoomData room, Vector3Int origin)
    {
        DrawFloorAndWalls(room, origin);
        DrawDoors(room, origin);
        DrawObstacles(room, origin);  // ��ֹ�(���) �׸��� �߰�
        // TODO: �߰� ��ֹ��̳� ��Ĺ� ����
    }

    // �ٴڰ� �� Ÿ�� �׸���
    private void DrawFloorAndWalls(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);
                floorTilemap.SetTile(tilePos, floorTile);

                // �� �׵θ��� �� Ÿ�Ϸ� ó��
                if (IsBorder(x, y, room.tileSize))
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }
    }

    // �� Ÿ�ϰ� �� ������ ����
    private void DrawDoors(RoomData room, Vector3Int origin)
    {
        var roomLoader = FindAnyObjectByType<RoomLoader>();
        if (roomLoader == null || doorPrefab == null)
            return;

        foreach (Vector2Int localPos in room.doorLocalPositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // �� Ÿ���� �� Ÿ�Ϸ� ��ü
            if (doorTile != null)
                wallTilemap.SetTile(tilePos, doorTile);

            // �� ������ ��ġ ���� (Ÿ�� �߽�)
            Vector3 worldPos = wallTilemap.CellToWorld(tilePos) + new Vector3(0, 0, 0);

            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);
            if (door.TryGetComponent(out DoorTrigger trigger))
            {
                trigger.direction = GetDirectionFromDoor(localPos, room.tileSize);
                trigger.roomLoader = roomLoader;
            }

            spawnedDoors.Add(door);
        }
    }

    // ��ֹ�(���) Ÿ�� �׸���
    private void DrawObstacles(RoomData room, Vector3Int origin)
    {
        if (room.obstaclePositions == null || room.obstaclePositions.Count == 0)
            return;

        foreach (var localPos in room.obstaclePositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // ��ֹ��� �� Ÿ�Ϸ� ǥ�� (��� ����)
            wallTilemap.SetTile(tilePos, wallTile);
        }
    }

    private bool IsBorder(int x, int y, Vector2Int size)
    {
        return x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
    }

    private Vector2Int GetDirectionFromDoor(Vector2Int localPos, Vector2Int tileSize)
    {
        if (localPos.x == 0) return Vector2Int.left;
        if (localPos.x == tileSize.x - 1) return Vector2Int.right;
        if (localPos.y == 0) return Vector2Int.down;
        if (localPos.y == tileSize.y - 1) return Vector2Int.up;
        return Vector2Int.zero;
    }

    // ��� �� ���� ȣ��
    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
        {
            if (door != null && door.TryGetComponent(out DoorTrigger trigger))
                trigger.Open();
        }
    }
}
