using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

/// <summary>
/// �� ���� ��� ��(Room)�� �����ϴ� Ŭ����.
/// �ʱ�ȭ �� MapGenerator�� ���� ����� �����ϰ�, ��ġ �������� ��ȸ ����.
/// </summary>
public class RoomManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int roomCount = 15;          // ������ �� ����
    public int roomUnitSize = 6;        // �� ���� �� ���� ũ�� (tilePerCell)

    private Dictionary<Vector2Int, RoomData> rooms;  // ���� ������ ����� �����ϴ� �� (��ǥ -> RoomData)

    /// <summary>
    /// �ܺο��� rooms�� �б� �������� ������ �� �ֵ��� ����
    /// </summary>
    public IReadOnlyDictionary<Vector2Int, RoomData> Rooms => rooms;

    /// <summary>
    /// ���� ���� �� ���� ����
    /// </summary>
    private void Awake()
    {
        // �� ������ �ν��Ͻ� ���� (���� �� ũ�⸦ ����)
        var generator = new MapGenerator(roomUnitSize);

        // roomCount ����ŭ ���� �����ϰ� ����
        rooms = generator.GenerateMap(roomCount);
    }

    /// <summary>
    /// Ư�� ��ǥ�� �ش��ϴ� ���� ��ȯ (������ null)
    /// </summary>
    /// <param name="position">��ȸ�� ��ǥ</param>
    public RoomData GetRoomAt(Vector2Int position)
    {
        return rooms.TryGetValue(position, out var room) ? room : null;
    }
}
