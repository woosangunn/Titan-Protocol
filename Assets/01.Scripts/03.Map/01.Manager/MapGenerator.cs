using MyGame.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    private Dictionary<Vector2Int, RoomData> roomMap = new();

    private int maxRooms;
    private int tilePerCellX;
    private int tilePerCellY;

    private readonly Vector2Int minCellSize = new(3, 3);
    private readonly Vector2Int maxCellSize = new(4, 4);

    private static readonly List<Vector2Int> directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private const float doorProximityThreshold = 1.5f;

    public MapGenerator(int roomUnitSize)
    {
        tilePerCellX = roomUnitSize;
        tilePerCellY = roomUnitSize;
    }

    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        Queue<Vector2Int> queue = new();
        Vector2Int startPos = Vector2Int.zero;

        RoomData startRoom = CreateRoom(RoomType.Start, startPos, new Vector2Int(2, 2));
        roomMap[startPos] = startRoom;
        queue.Enqueue(startPos);

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

                ConnectRooms(roomMap[current], room, dir);

                if (roomMap.Count >= maxRooms) break;
            }
        }

        foreach (var room in roomMap.Values)
        {
            GenerateObstacles(room);
        }

        return roomMap;
    }

    private RoomData CreateRoom(RoomType type, Vector2Int cellPos, Vector2Int cellSize)
    {
        Vector2Int tileSize = new(cellSize.x * tilePerCellX, cellSize.y * tilePerCellY);

        RoomData room = new RoomData
        {
            type = type,
            cellPosition = cellPos,
            cellSize = cellSize,
            tileSize = tileSize
        };

        if (type == RoomType.Combat)
        {
            Vector2 center = new Vector2(tileSize.x, tileSize.y) * 0.5f;
            room.enemySpawnPoints.Add(center);
        }

        return room;
    }

    private Vector2Int GetRandomCellSize()
    {
        int w = Random.Range(minCellSize.x, maxCellSize.x + 1);
        int h = Random.Range(minCellSize.y, maxCellSize.y + 1);
        return new Vector2Int(w, h);
    }

    private void ConnectRooms(RoomData roomA, RoomData roomB, Vector2Int dir)
    {
        Vector2Int GetDoorPos(Vector2Int size, Vector2Int d) => d switch
        {
            var v when v == Vector2Int.up => new Vector2Int(size.x / 2, size.y - 1),
            var v when v == Vector2Int.down => new Vector2Int(size.x / 2, 0),
            var v when v == Vector2Int.left => new Vector2Int(0, size.y / 2),
            var v when v == Vector2Int.right => new Vector2Int(size.x - 1, size.y / 2),
            _ => new Vector2Int(size.x / 2, size.y / 2)
        };

        roomA.doors.Add(new DoorData
        {
            localPosition = GetDoorPos(roomA.tileSize, dir),
            direction = dir,
            connectedRoomPos = roomB.cellPosition
        });

        roomB.doors.Add(new DoorData
        {
            localPosition = GetDoorPos(roomB.tileSize, -dir),
            direction = -dir,
            connectedRoomPos = roomA.cellPosition
        });
    }

    private void GenerateObstacles(RoomData room)
    {
        room.obstaclePositions.Clear();

        if (room.type is RoomType.Start or RoomType.Item or RoomType.Boss or RoomType.Shop)
            return;

        Vector2Int size = room.tileSize;
        List<Vector2Int> potentialPositions = new();

        int offsetX = Mathf.Clamp(Mathf.RoundToInt(size.x * 0.3f), 2, size.x - 3);
        int offsetY = Mathf.Clamp(Mathf.RoundToInt(size.y * 0.3f), 2, size.y - 3);

        potentialPositions.Add(new Vector2Int(offsetX, offsetY));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, offsetY));
        potentialPositions.Add(new Vector2Int(offsetX, size.y - offsetY - 1));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, size.y - offsetY - 1));

        if (size.x >= 20 && size.y >= 20)
            potentialPositions.Add(new Vector2Int(size.x / 2, size.y / 2));

        potentialPositions.RemoveAll(pos => IsNearDoor(pos, room.doors.Select(d => d.localPosition)));

        int count = Mathf.Min(Random.Range(2, 5), potentialPositions.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, potentialPositions.Count);
            room.obstaclePositions.Add(potentialPositions[index]);
            potentialPositions.RemoveAt(index);
        }
    }

    private bool IsNearDoor(Vector2Int pos, IEnumerable<Vector2Int> doorPositions)
    {
        foreach (var door in doorPositions)
        {
            if (Vector2Int.Distance(pos, door) < doorProximityThreshold)
                return true;
        }
        return false;
    }
}
