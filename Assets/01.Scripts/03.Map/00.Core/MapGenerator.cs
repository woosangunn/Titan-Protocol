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

        SetNeighborsAndDoors(); // 문 위치 및 인접 방 방향 설정 꼭 호출 필요

        return roomMap;
    }

    private void PlaceMandatoryRooms()
    {
        // 시작방 무조건 (0,0)
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

    private Vector2Int GetRandomSize()
    {
        int width = Random.Range(10, 20);
        int height = Random.Range(8, 16);
        return new Vector2Int(width, height);
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
        return new Vector2Int(1, 0); // 실패 시 기본값
    }

    private void SetNeighborsAndDoors()
    {
        foreach (var kvp in roomMap)
        {
            RoomData room = kvp.Value;
            room.neighborDirs.Clear();
            room.doorLocalPositions.Clear();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = room.position + dir;
                if (roomMap.ContainsKey(neighborPos))
                {
                    room.neighborDirs.Add(dir);

                    Vector2Int doorLocalPos = GetDoorLocalPosition(room.size, dir);
                    room.doorLocalPositions.Add(doorLocalPos);
                }
            }

            Debug.Log($"[MapGenerator] Room at {room.position} has {room.doorLocalPositions.Count} doors.");
        }
    }

    private Vector2Int GetDoorLocalPosition(Vector2Int roomSize, Vector2Int dir)
    {
        Vector2Int center = roomSize / 2;

        if (dir == Vector2Int.up)
            return new Vector2Int(center.x, roomSize.y - 1);
        else if (dir == Vector2Int.down)
            return new Vector2Int(center.x, 0);
        else if (dir == Vector2Int.left)
            return new Vector2Int(0, center.y);
        else if (dir == Vector2Int.right)
            return new Vector2Int(roomSize.x - 1, center.y);

        return center;
    }
}
