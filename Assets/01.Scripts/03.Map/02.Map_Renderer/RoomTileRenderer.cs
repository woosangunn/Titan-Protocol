using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using MyGame.Map;

public class RoomTileRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;       // �� �ٴ��� ǥ���� Tilemap
    public Tilemap wallTilemap;        // �� ��(�Ǵ� ��ֹ�)�� ǥ���� Tilemap

    [Header("Tiles")]
    public TileBase floorTile;         // �ٴڿ� ����� Ÿ��
    public TileBase wallTile;          // ���� ����� Ÿ��
    public TileBase doorTile;          // ���� ����� Ÿ�� (���� Ÿ�� ���·� ǥ���� �� ���)

    [Header("Prefabs")]
    public GameObject doorPrefab;      // �� ������Ʈ ������ (�� Ʈ���� ����)

    private readonly List<GameObject> spawnedDoors = new(); // ������ �� ������Ʈ���� �����ϴ� ����Ʈ

    // ���� ���� Ÿ�� �� �� ������Ʈ ����
    public void ClearTiles()
    {
        // �ٴ� �� �� Ÿ�� ����
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // ������ �� ������Ʈ ����
        foreach (var door in spawnedDoors)
        {
            if (door != null)
                Destroy(door);
        }
        spawnedDoors.Clear(); // ����Ʈ �ʱ�ȭ
    }

    // ���� �����ϴ� ��� ��Ҹ� �׸��� �Լ�
    public void DrawRoom(RoomData room, Vector3Int origin)
    {
        DrawFloorAndWalls(room, origin); // �ٴ� �� �� Ÿ��
        DrawDoors(room, origin);         // �� Ÿ�� + ������
        DrawObstacles(room, origin);     // ��� �� ��ֹ�
        // ���� �ٸ� ��� �߰� ���� (TODO)
    }

    // �� ���� �ٴ� �� �ܰ� �� �׸���
    private void DrawFloorAndWalls(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);

                // �ٴ� Ÿ�� ����
                floorTilemap.SetTile(tilePos, floorTile);

                // �׵θ���� �� Ÿ�Ϸ� ����
                if (IsBorder(x, y, room.tileSize))
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }
    }

    // �� ���� (Ÿ�� + ������)
    private void DrawDoors(RoomData room, Vector3Int origin)
    {
        var roomLoader = FindAnyObjectByType<RoomLoader>(); // �� ���ۿ� RoomLoader Ž��
        if (roomLoader == null || doorPrefab == null)
            return;

        foreach (Vector2Int localPos in room.doorLocalPositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // �� Ÿ�� ���� (���� ���� ������ ��ü)
            if (doorTile != null)
                wallTilemap.SetTile(tilePos, doorTile);

            // �� ������ ��ġ (��ġ ���� ����)
            Vector3 worldPos = wallTilemap.CellToWorld(tilePos) + new Vector3(0, 0, 0); // ���߾� ��ġ

            // �� ������ ���� �� ���� ����
            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);
            if (door.TryGetComponent(out DoorTrigger trigger))
            {
                trigger.direction = GetDirectionFromDoor(localPos, room.tileSize); // �� ���� ����
                trigger.roomLoader = roomLoader; // ���� ��δ� ����
            }

            spawnedDoors.Add(door); // ������ ����Ʈ�� ���
        }
    }

    // ��ֹ�(��� ��)�� �׸��� �Լ�
    private void DrawObstacles(RoomData room, Vector3Int origin)
    {
        if (room.obstaclePositions == null || room.obstaclePositions.Count == 0)
            return;

        foreach (var localPos in room.obstaclePositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // ��ֹ��� �� Ÿ���� ��Ȱ�� (������ �ʿ��ϸ� ���� Ÿ�� �ʿ�)
            wallTilemap.SetTile(tilePos, wallTile);
        }
    }

    // �ش� ��ġ�� ���� �׵θ����� Ȯ�� (0�̳� �� ��ǥ)
    private bool IsBorder(int x, int y, Vector2Int size)
    {
        return x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
    }

    // �� ��ġ(localPos) �������� ���� ���� ���
    private Vector2Int GetDirectionFromDoor(Vector2Int localPos, Vector2Int tileSize)
    {
        if (localPos.x == 0) return Vector2Int.left;
        if (localPos.x == tileSize.x - 1) return Vector2Int.right;
        if (localPos.y == 0) return Vector2Int.down;
        if (localPos.y == tileSize.y - 1) return Vector2Int.up;
        return Vector2Int.zero; // ���� ó�� (�� ���ο� ���� �ִٸ� ���� ����)
    }

    // ��� �� ������Ʈ�� ������ ���
    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
        {
            if (door != null && door.TryGetComponent(out DoorTrigger trigger))
                trigger.Open();
        }
    }
}
