using System.Collections.Generic;
using UnityEngine;
using MyGame.Map;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public int roomCount = 10;                            // 생성할 방 개수
    public Vector2Int tileOffsetStep = new Vector2Int(50, 40);  // 방 간 간격 (타일 단위)

    private Dictionary<Vector2Int, RoomData> roomMap;    // 위치별 방 데이터 맵
    private Vector2Int currentPos;                        // 현재 플레이어가 위치한 방 좌표

    private RoomTileRenderer tileRenderer;

    private Vector3Int lastRoomOffset;
    private Vector2 lastRoomSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        tileRenderer = GetComponent<RoomTileRenderer>();
    }

    private void Start()
    {
        // 1. 맵 생성
        roomMap = new MapGenerator().GenerateMap(roomCount);

        // 2. 현재 위치는 Start 방 위치 (0,0) 가정
        currentPos = Vector2Int.zero;

        // 3. 모든 방 타일맵에 그리기 (복도 없이 방만)
        DrawAllRooms();

        // 4. 플레이어 시작 위치 조정 (방 중심 기준)
        PositionPlayerToCurrentRoomCenter();

        // 5. 시작 방 상태 갱신
        SetRoomDiscovered(currentPos);
    }

    /// <summary>
    /// 모든 방을 타일맵에 그린다 (복도 없음)
    /// </summary>
    private void DrawAllRooms()
    {
        if (tileRenderer == null) return;

        foreach (var kvp in roomMap)
        {
            RoomData room = kvp.Value;
            Vector3Int offset = new Vector3Int(room.position.x * tileOffsetStep.x, room.position.y * tileOffsetStep.y, 0);

            tileRenderer.DrawRoom(room, offset);
        }
    }

    /// <summary>
    /// 플레이어가 현재 방 중심으로 위치하게 함
    /// </summary>
    private void PositionPlayerToCurrentRoomCenter()
    {
        RoomData currentRoom = GetRoomData(currentPos);
        if (currentRoom == null)
        {
            Debug.LogWarning("현재 방 데이터가 없습니다.");
            return;
        }

        // 방 타일 좌표 기준으로 플레이어 위치 계산 (중앙)
        Vector3Int offset = new Vector3Int(currentPos.x * tileOffsetStep.x, currentPos.y * tileOffsetStep.y, 0);
        Vector2 roomCenter = new Vector2(offset.x + currentRoom.size.x / 2f, offset.y + currentRoom.size.y / 2f);

        // 플레이어 오브젝트를 찾고 위치 이동 (태그 "Player"를 가정)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(roomCenter.x, roomCenter.y, player.transform.position.z);
        }
    }

    /// <summary>
    /// 특정 방향으로 방 이동 시도
    /// </summary>
    /// <param name="dir">이동 방향 (Vector2Int.up, down, left, right)</param>
    public void TryMove(Vector2Int dir)
    {
        Vector2Int nextPos = currentPos + dir;
        if (!roomMap.ContainsKey(nextPos))
        {
            Debug.Log("이동할 방이 없습니다.");
            return;
        }

        LoadRoom(nextPos);
    }

    /// <summary>
    /// 방을 로드하고 플레이어 위치 갱신
    /// </summary>
    /// <param name="pos">로드할 방 좌표</param>
    private void LoadRoom(Vector2Int pos)
    {
        RoomData room = GetRoomData(pos);
        if (room == null)
        {
            Debug.LogWarning("존재하지 않는 방 위치입니다.");
            return;
        }

        // 이전 방 클리어는 여기선 생략 (전체 맵 그리므로)

        currentPos = pos;

        // 플레이어 위치 이동
        PositionPlayerToCurrentRoomCenter();

        // 방 방문 상태 갱신
        SetRoomDiscovered(pos);

        Debug.Log($"현재 방 위치: {pos}, 타입: {room.type}");
    }

    /// <summary>
    /// 방 방문 상태를 Discovered로 변경
    /// </summary>
    private void SetRoomDiscovered(Vector2Int pos)
    {
        RoomData room = GetRoomData(pos);
        if (room != null && room.state == RoomState.Unvisited)
        {
            room.state = RoomState.Discovered;
        }
    }

    /// <summary>
    /// 특정 방 데이터 반환 (없으면 null)
    /// </summary>
    public RoomData GetRoomData(Vector2Int pos)
    {
        if (roomMap != null && roomMap.TryGetValue(pos, out RoomData room))
            return room;
        return null;
    }

    /// <summary>
    /// 현재 플레이어가 위치한 방 좌표 반환
    /// </summary>
    public Vector2Int GetCurrentPosition()
    {
        return currentPos;
    }
}
