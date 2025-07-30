using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

public class RoomManager : MonoBehaviour
{
    public int roomCount = 10;
    public Vector2Int roomSizeMin = new(10, 10);
    public Vector2Int roomSizeMax = new(12, 12);

    private Dictionary<Vector2Int, RoomData> rooms = new();

    public IReadOnlyDictionary<Vector2Int, RoomData> Rooms => rooms;

    private void Awake()
    {
        // MapGenerator가 방들을 생성해 rooms 딕셔너리에 저장
        rooms = new MapGenerator().GenerateMap(roomCount);

        // 디버그: 생성된 방 위치와 문 개수 출력
        foreach (var kvp in rooms)
        {
            Debug.Log($"[RoomManager] Room at {kvp.Key} 생성, 문 수: {kvp.Value.doorLocalPositions.Count}");
        }
    }

    public RoomData GetRoomAt(Vector2Int position)
    {
        return rooms.TryGetValue(position, out var room) ? room : null;
    }

    public Vector2Int GetRoomSize(Vector2Int position)
    {
        return rooms.TryGetValue(position, out var room) ? room.size : new Vector2Int(20, 20);
    }
}
