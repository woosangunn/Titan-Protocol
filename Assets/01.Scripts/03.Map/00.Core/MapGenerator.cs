using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    private Vector2Int[] directions = new[] {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        var map = new Dictionary<Vector2Int, RoomData>();
        Vector2Int startPos = Vector2Int.zero;

        RoomData start = new RoomData { position = startPos, type = RoomType.Start };
        map[startPos] = start;

        Stack<RoomData> stack = new();
        stack.Push(start);

        while (map.Count < maxRooms && stack.Count > 0)
        {
            RoomData current = stack.Pop();
            Shuffle(directions);

            foreach (var dir in directions)
            {
                Vector2Int nextPos = current.position + dir;
                if (map.ContainsKey(nextPos)) continue;

                RoomType type = RoomType.Combat;

                if (map.Count == maxRooms - 1)
                    type = RoomType.Boss;
                else if (Random.value < 0.1f)
                    type = RoomType.Treasure;
                else if (Random.value < 0.05f)
                    type = RoomType.Shop;

                RoomData newRoom = new RoomData
                {
                    position = nextPos,
                    type = type
                };

                map[nextPos] = newRoom;
                current.neighborDirs.Add(dir);
                newRoom.neighborDirs.Add(-dir);
                stack.Push(newRoom);
                break;
            }
        }

        return map;
    }

    void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rand = Random.Range(i, array.Length);
            (array[i], array[rand]) = (array[rand], array[i]);
        }
    }
}
