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
        // MapGenerator�� ����� ������ rooms ��ųʸ��� ����
        rooms = new MapGenerator().GenerateMap(roomCount);

        // �����: ������ �� ��ġ�� �� ���� ���
        foreach (var kvp in rooms)
        {
            Debug.Log($"[RoomManager] Room at {kvp.Key} ����, �� ��: {kvp.Value.doorLocalPositions.Count}");
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
