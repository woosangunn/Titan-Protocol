using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

public class MapGenerator
{
    private Dictionary<Vector2Int, RoomData> roomMap = new();
    private int maxRooms;

    private List<RoomType> mandatoryRooms = new() { RoomType.Start, RoomType.Shop, RoomType.Item };
    private List<RoomType> normalRooms = new() { RoomType.Combat };

    private static readonly List<Vector2Int> directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        PlaceMandatoryRooms();
        GenerateNormalRooms();
        SetNeighbors();

        return roomMap;
    }

    private void PlaceMandatoryRooms()
    {
        RoomData startRoom = CreateRoom(RoomType.Start, Vector2Int.zero);
        roomMap.Add(Vector2Int.zero, startRoom);

        foreach (RoomType roomType in mandatoryRooms)
        {
            if (roomType == RoomType.Start) continue;

            Vector2Int pos = FindRandomAvailablePosition();
            RoomData room = CreateRoom(roomType, pos);
            roomMap.Add(pos, room);
        }
    }

    private void GenerateNormalRooms()
    {
        int roomsToCreate = maxRooms - roomMap.Count;
        for (int i = 0; i < roomsToCreate; i++)
        {
            Vector2Int pos = FindRandomAvailablePosition();
            RoomType roomType = GetRandomNormalRoomType();

            RoomData room = CreateRoom(roomType, pos);
            roomMap.Add(pos, room);
        }
    }

    private RoomData CreateRoom(RoomType type, Vector2Int position)
    {
        RoomData room = new RoomData();
        room.type = type;
        room.position = position;
        room.size = GetRandomSize();
        room.state = RoomState.Unvisited;
        return room;
    }

    private Vector2 GetRandomSize()
    {
        int width = Random.Range(10, 20);
        int height = Random.Range(8, 16);
        return new Vector2(width, height);
    }

    private RoomType GetRandomNormalRoomType()
    {
        int index = Random.Range(0, normalRooms.Count);
        return normalRooms[index];
    }

    private Vector2Int FindRandomAvailablePosition()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            int x = Random.Range(-10, 10);
            int y = Random.Range(-10, 10);
            Vector2Int pos = new(x, y);
            if (!roomMap.ContainsKey(pos))
                return pos;
        }
        return new Vector2Int(1, 0);
    }

    private void SetNeighbors()
    {
        foreach (var kvp in roomMap)
        {
            RoomData room = kvp.Value;
            room.neighborDirs.Clear();

            foreach (Vector2Int dir in directions)
            {
                if (roomMap.ContainsKey(room.position + dir))
                    room.neighborDirs.Add(dir);
            }
        }
    }
}
