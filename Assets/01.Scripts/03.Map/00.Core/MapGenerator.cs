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

    // 생성자 추가 (셀 단위 한 블록 크기 받음)
    public MapGenerator(int roomUnitSize)
    {
        tilePerCellX = roomUnitSize;
        tilePerCellY = roomUnitSize;
    }

    // 기본 생성자도 필요하면 추가 (기본 10으로 설정)
    public MapGenerator()
    {
        tilePerCellX = 10;
        tilePerCellY = 10;
    }

    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        Queue<Vector2Int> queue = new();
        queue.Enqueue(Vector2Int.zero);
        roomMap[Vector2Int.zero] = CreateRoom(RoomType.Start, Vector2Int.zero, GetRandomCellSize());

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

        SetNeighborsAndDoors();
        return roomMap;
    }

    private RoomData CreateRoom(RoomType type, Vector2Int cellPos, Vector2Int cellSize)
    {
        return new RoomData
        {
            type = type,
            cellPosition = cellPos,
            cellSize = cellSize,
            tileSize = new Vector2Int(cellSize.x * tilePerCellX, cellSize.y * tilePerCellY)
        };
    }

    private Vector2Int GetRandomCellSize()
    {
        int w = Random.Range(1, 3); // 1~2 셀
        int h = Random.Range(1, 3);
        return new Vector2Int(w, h);
    }

    private void SetNeighborsAndDoors()
    {
        foreach (var room in roomMap.Values)
        {
            room.neighborDirs.Clear();
            room.doorLocalPositions.Clear();

            foreach (var dir in directions)
            {
                Vector2Int neighbor = room.cellPosition + dir;
                if (roomMap.ContainsKey(neighbor))
                {
                    room.neighborDirs.Add(dir);
                    room.doorLocalPositions.Add(GetDoorLocalPosition(room.tileSize, dir));
                }
            }
        }
    }

    private Vector2Int GetDoorLocalPosition(Vector2Int tileSize, Vector2Int dir)
    {
        Vector2Int center = tileSize / 2;
        if (dir == Vector2Int.up) return new Vector2Int(center.x, tileSize.y - 1);
        if (dir == Vector2Int.down) return new Vector2Int(center.x, 0);
        if (dir == Vector2Int.left) return new Vector2Int(0, center.y);
        if (dir == Vector2Int.right) return new Vector2Int(tileSize.x - 1, center.y);
        return center;
    }
}
