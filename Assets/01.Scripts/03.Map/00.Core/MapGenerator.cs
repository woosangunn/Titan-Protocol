using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

public class MapGenerator
{
    private Dictionary<Vector2Int, RoomData> roomMap = new();
    private int maxRooms;

    private static readonly List<Vector2Int> directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private int tilePerCellX;
    private int tilePerCellY;
    private readonly Vector2Int minCellSize = new(2, 2);
    private readonly Vector2Int maxCellSize = new(3, 3);

    public MapGenerator(int roomUnitSize)
    {
        tilePerCellX = roomUnitSize;
        tilePerCellY = roomUnitSize;
    }

    // �ִ� �� ������ ���� �� ����
    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        Queue<Vector2Int> queue = new();
        Vector2Int startPos = Vector2Int.zero;

        // ���� �� ����(ũ�� ����)
        RoomData startRoom = CreateRoom(RoomType.Start, startPos, new Vector2Int(2, 2));
        roomMap[startPos] = startRoom;
        queue.Enqueue(startPos);

        // BFS ������� �ֺ� �� ����
        while (roomMap.Count < maxRooms && queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (roomMap.ContainsKey(next)) continue;
                if (Random.value > 0.5f) continue;

                RoomData room = CreateRoom(RoomType.Combat, next, GetRandomCellSize());
                roomMap[next] = room;
                queue.Enqueue(next);

                if (roomMap.Count >= maxRooms) break;
            }
        }

        // �̿� ���� �� �� ��ġ ����
        SetNeighborsAndDoors();

        // ��ֹ�(���) ��ġ ����
        foreach (var room in roomMap.Values)
        {
            GenerateObstacles(room);
        }

        return roomMap;
    }

    // �� ���� �� Ÿ�� ũ�� ���
    private RoomData CreateRoom(RoomType type, Vector2Int cellPos, Vector2Int cellSize)
    {
        if (type == RoomType.Start)
            cellSize = new Vector2Int(2, 2);

        Vector2Int tileSize = new(cellSize.x * tilePerCellX, cellSize.y * tilePerCellY);

        return new RoomData
        {
            type = type,
            cellPosition = cellPos,
            cellSize = cellSize,
            tileSize = tileSize
        };
    }

    // �� ũ�� ���� ����
    private Vector2Int GetRandomCellSize()
    {
        int w = Random.Range(minCellSize.x, maxCellSize.x + 1);
        int h = Random.Range(minCellSize.y, maxCellSize.y + 1);
        return new Vector2Int(w, h);
    }

    // �̿� ���� �� �� ��ġ ���
    private void SetNeighborsAndDoors()
    {
        foreach (var room in roomMap.Values)
        {
            room.neighborDirs.Clear();
            room.doorLocalPositions.Clear();

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = room.cellPosition + dir;
                if (roomMap.TryGetValue(neighborPos, out var neighbor))
                {
                    room.neighborDirs.Add(dir);
                    var doorPos = GetDoorLocalPosition(room.tileSize, dir);
                    room.doorLocalPositions.Add(doorPos);
                }
            }
        }
    }

    // �� ��ġ ��� (�� Ÿ�� ũ�� ����)
    private Vector2Int GetDoorLocalPosition(Vector2Int tileSize, Vector2Int dir)
    {
        int centerX = tileSize.x / 2;
        int centerY = tileSize.y / 2;

        return dir switch
        {
            var d when d == Vector2Int.up => new Vector2Int(centerX, tileSize.y - 1),
            var d when d == Vector2Int.down => new Vector2Int(centerX, 0),
            var d when d == Vector2Int.left => new Vector2Int(0, centerY),
            var d when d == Vector2Int.right => new Vector2Int(tileSize.x - 1, centerY),
            _ => new Vector2Int(centerX, centerY)
        };
    }

    // ��ֹ�(���) ��ġ ���� ����, �ߺ� �� �� ��ġ ���� ó��
    private void GenerateObstacles(RoomData room)  // ���� ��ֹ� ��ġ �ʱ�ȭ
    {
        room.obstaclePositions.Clear();

        int numObstacles = Random.Range(2, 5); // 2~4�� ������ ��ֹ� ���� ������ ����
        Vector2Int center = new(room.tileSize.x / 2, room.tileSize.y / 2); // ���� �߾� ��ǥ ���

        int attempts = 0;
        int maxAttempts = 30; // ���� ���� ������ ���� (�浹/���� ������ ��ֹ� �� ���� �� �����Ƿ�)

        while (room.obstaclePositions.Count < numObstacles && attempts < maxAttempts)
        {
            // �� ���� (�׵θ� ����)���� ���� ��ǥ ����
            Vector2Int pos = new Vector2Int(
                Random.Range(1, room.tileSize.x - 1),
                Random.Range(1, room.tileSize.y - 1)
            );

            // �߾Ӱ� �ʹ� ������ ���� (�÷��̾� ���� ���� Ȯ����)
            if (Vector2Int.Distance(pos, center) < 2f) { attempts++; continue; }

            // �� ��ġ �Ǵ� �� ��ó(1.5ĭ �̳�)�� ���� (���Թ� ���� �ʵ���)
            if (IsNearDoor(pos, room.doorLocalPositions)) { attempts++; continue; }

            // �̹� ���� ��ġ�� ��ֹ��� ������ ���� (�ߺ� ����)
            if (room.obstaclePositions.Contains(pos)) { attempts++; continue; }

            // �� ���� �� ����ϸ� ��ֹ� ��ġ�� ���
            room.obstaclePositions.Add(pos);
            attempts++;
        }
    }

    // �� �ֺ� 1.5ĭ �̳� �Ÿ��� true
    private bool IsNearDoor(Vector2Int pos, List<Vector2Int> doors)
    {
        foreach (var door in doors)
        {
            if (Vector2Int.Distance(pos, door) < 1.5f)
                return true;
        }
        return false;
    }
}
