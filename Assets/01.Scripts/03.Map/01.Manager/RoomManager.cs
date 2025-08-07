using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

/// <summary>
/// 맵 상의 모든 방(Room)을 관리하는 클래스.
/// 초기화 시 MapGenerator를 통해 방들을 생성하고, 위치 기준으로 조회 가능.
/// </summary>
public class RoomManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int roomCount = 15;          // 생성할 방 개수
    public int roomUnitSize = 6;        // 각 방의 셀 단위 크기 (tilePerCell)

    private Dictionary<Vector2Int, RoomData> rooms;  // 실제 생성된 방들을 저장하는 맵 (좌표 -> RoomData)

    /// <summary>
    /// 외부에서 rooms를 읽기 전용으로 접근할 수 있도록 노출
    /// </summary>
    public IReadOnlyDictionary<Vector2Int, RoomData> Rooms => rooms;

    /// <summary>
    /// 게임 시작 시 맵을 생성
    /// </summary>
    private void Awake()
    {
        // 맵 생성기 인스턴스 생성 (단위 셀 크기를 전달)
        var generator = new MapGenerator(roomUnitSize);

        // roomCount 수만큼 방을 생성하고 저장
        rooms = generator.GenerateMap(roomCount);
    }

    /// <summary>
    /// 특정 좌표에 해당하는 방을 반환 (없으면 null)
    /// </summary>
    /// <param name="position">조회할 좌표</param>
    public RoomData GetRoomAt(Vector2Int position)
    {
        return rooms.TryGetValue(position, out var room) ? room : null;
    }
}
