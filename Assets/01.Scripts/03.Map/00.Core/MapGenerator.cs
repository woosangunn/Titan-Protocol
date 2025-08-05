using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

public class MapGenerator
{
    // ������ ����� �����ϴ� �� (key: �� ��ǥ, value: �� ������)
    private Dictionary<Vector2Int, RoomData> roomMap = new();

    // ������ �ִ� �� ��
    private int maxRooms;

    // �� ���� ���� (��, ��, ��, ��)
    private static readonly List<Vector2Int> directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    // �� �ϳ��� Ÿ�� �� (ũ�� ������)
    private int tilePerCellX;
    private int tilePerCellY;

    // �� ũ���� �ּ�/�ִ� ����
    private readonly Vector2Int minCellSize = new(3, 3);
    private readonly Vector2Int maxCellSize = new(4, 4); // �� ū ���� ����� ���� Ȯ�� ����

    // ������: �� �ϳ��� Ÿ�� �� ����
    public MapGenerator(int roomUnitSize)
    {
        tilePerCellX = roomUnitSize;
        tilePerCellY = roomUnitSize;
    }

    // ��ü �� ���� �Լ� (�ִ� �� ����ŭ ����)
    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        Queue<Vector2Int> queue = new();             // BFS ó���� ���� ť
        Vector2Int startPos = Vector2Int.zero;       // ���� �� ��ġ

        // ���� �� ���� (ũ�� ����)
        RoomData startRoom = CreateRoom(RoomType.Start, startPos, new Vector2Int(2, 2));
        roomMap[startPos] = startRoom;
        queue.Enqueue(startPos);

        // BFS ������� ������ �� ����
        while (roomMap.Count < maxRooms && queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                // �̹� ������ ��ġ�� ��ŵ
                if (roomMap.ContainsKey(next)) continue;

                // ���� Ȯ���� �� ���� ���� ����
                if (Random.value > 0.5f) continue;

                // ���ο� ���� �� ����
                RoomData room = CreateRoom(RoomType.Combat, next, GetRandomCellSize());
                roomMap[next] = room;
                queue.Enqueue(next);

                // �ִ� �� �� ���� �� �ߴ�
                if (roomMap.Count >= maxRooms) break;
            }
        }

        // �� ���� �̿� ����� �� ��ġ ���
        SetNeighborsAndDoors();

        // �� �濡 ��ֹ� ��ġ ����
        foreach (var room in roomMap.Values)
        {
            GenerateObstacles(room);
        }

        return roomMap;
    }

    // �� ���� �Լ� (Ÿ�� ũ�� ��� ����)
    private RoomData CreateRoom(RoomType type, Vector2Int cellPos, Vector2Int cellSize)
    {
        // �� ũ�⸦ ������� ���� Ÿ�� ũ�� ���
        Vector2Int tileSize = new(cellSize.x * tilePerCellX, cellSize.y * tilePerCellY);

        return new RoomData
        {
            type = type,
            cellPosition = cellPos,     // �� ��ǥ
            cellSize = cellSize,        // �� ũ�� (���� ����)
            tileSize = tileSize         // ���� Ÿ�� ũ��
        };
    }

    // �� ũ�⸦ �������� ���� (min~max ���� ��)
    private Vector2Int GetRandomCellSize()
    {
        int w = Random.Range(minCellSize.x, maxCellSize.x + 1);
        int h = Random.Range(minCellSize.y, maxCellSize.y + 1);
        return new Vector2Int(w, h);
    }

    // �� �� ���� ���� �� �� ��ġ ����
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
                    // �̿� ���� ���
                    room.neighborDirs.Add(dir);

                    // �ش� ���⿡ �� ��ġ �߰�
                    var doorPos = GetDoorLocalPosition(room.tileSize, dir);
                    room.doorLocalPositions.Add(doorPos);
                }
            }
        }
    }

    // �� ��ġ ��� �Լ� (���� Ÿ�� ũ��� ������ �������� �߾� �� ��ġ ����)
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
            _ => new Vector2Int(centerX, centerY) // �⺻�� (�߾�)
        };
    }

    // �� ���ο� ��� ��ֹ� ���� (���� ��ġ�� �ʵ���)
    private void GenerateObstacles(RoomData room)
    {
        room.obstaclePositions.Clear();

        // Ư�� ���� ��� ���� ����
        if (room.type == RoomType.Start || room.type == RoomType.Item || room.type == RoomType.Boss || room.type == RoomType.Shop)
            return;

        Vector2Int size = room.tileSize;
        List<Vector2Int> potentialPositions = new();

        // 1. �� ������ ��� �ĺ� ��ġ �߰� (30% ��ġ ����)
        int offsetX = Mathf.RoundToInt(size.x * 0.3f);
        int offsetY = Mathf.RoundToInt(size.y * 0.3f);

        // �ʹ� �𼭸��� ���� �ʵ��� ����
        offsetX = Mathf.Clamp(offsetX, 2, size.x - 3);
        offsetY = Mathf.Clamp(offsetY, 2, size.y - 3);

        // ��Ī ��ġ�� ��� ��ġ
        potentialPositions.Add(new Vector2Int(offsetX, offsetY));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, offsetY));
        potentialPositions.Add(new Vector2Int(offsetX, size.y - offsetY - 1));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, size.y - offsetY - 1));

        // 2. ū ���̸� �߾� ��յ� �߰�
        if (size.x >= 20 && size.y >= 20)
            potentialPositions.Add(new Vector2Int(size.x / 2, size.y / 2));

        // 3. �� ��ġ�� ����� �ĺ� ���� (1.5ĭ �̳�)
        potentialPositions.RemoveAll(pos => IsNearDoor(pos, room.doorLocalPositions));

        // 4. �ִ� 2~4�� ���� �����Ͽ� ��� Ȯ��
        int count = Mathf.Min(Random.Range(2, 5), potentialPositions.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, potentialPositions.Count);
            room.obstaclePositions.Add(potentialPositions[index]);
            potentialPositions.RemoveAt(index); // �ߺ� ����
        }
    }

    // ����� ���� �ʹ� ������� �Ǵ� (�Ÿ� < 1.5�� true)
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
