using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

public class RoomManager : MonoBehaviour
{
    public int roomCount = 15;
    public int roomUnitSize = 6; // tilePerCell
    private Dictionary<Vector2Int, RoomData> rooms;

    public IReadOnlyDictionary<Vector2Int, RoomData> Rooms => rooms;

    private void Awake()
    {
        var generator = new MapGenerator(roomUnitSize);
        rooms = generator.GenerateMap(roomCount);
    }

    public RoomData GetRoomAt(Vector2Int position)
    {
        return rooms.TryGetValue(position, out var room) ? room : null;
    }
}