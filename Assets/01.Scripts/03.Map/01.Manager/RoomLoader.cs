using UnityEngine;
using MyGame.Map;

/// <summary>
/// 방 이동 및 방 데이터 로딩을 관리하는 컨트롤러 클래스
/// 방 이동 시 방 정보를 불러오고, 플레이어 위치 및 적 스폰 등을 조정함
/// </summary>
public class RoomLoader : MonoBehaviour
{
    [Header("References")]
    public RoomManager roomManager;                // 맵 내 모든 방을 관리하는 매니저
    public RoomRenderer roomRenderer;              // 방을 화면에 그리는 렌더러
    public PlayerPositioner playerPositioner;      // 플레이어 위치를 방 중심으로 이동시키는 역할
    public EnemySpawnController enemySpawnController;  // 전투 시작 및 적 스폰 담당

    [Header("현재 방 위치")]
    private Vector2Int currentRoomCoord = Vector2Int.zero;  // 현재 플레이어가 있는 방 좌표 (셀 단위)

    private void Start()
    {
        // 게임 시작 시 처음 방 로드
        LoadRoom(currentRoomCoord);
    }

    /// <summary>
    /// 플레이어가 특정 방향으로 방 이동을 시도할 때 호출
    /// </summary>
    /// <param name="dir">이동할 방향 벡터 (up/down/left/right)</param>
    public void TryMove(Vector2Int dir)
    {
        Vector2Int next = currentRoomCoord + dir;

        // 이동하려는 방이 존재하지 않으면 이동 불가 처리
        if (roomManager.GetRoomAt(next) == null)
        {
            Debug.LogWarning($"이동할 수 없는 방향: {dir}");
            return;
        }

        // 현재 방 좌표 갱신 및 해당 방 로드
        currentRoomCoord = next;
        LoadRoom(currentRoomCoord);
    }

    /// <summary>
    /// 특정 좌표의 방 데이터를 불러와 렌더링 및 상태에 따라 적 스폰, 문 개방 처리
    /// </summary>
    /// <param name="roomCoord">불러올 방의 좌표</param>
    private void LoadRoom(Vector2Int roomCoord)
    {
        RoomData room = roomManager.GetRoomAt(roomCoord);
        if (room == null) return;

        // 처음 방문하는 방이면 상태를 발견(Discovered)으로 변경
        if (room.state == RoomState.Unvisited)
            room.state = RoomState.Discovered;

        // 방 렌더링, 좌표 오프셋은 Vector3Int.zero (변경 가능)
        roomRenderer.Render(room, Vector3Int.zero, this);

        // 플레이어를 방 중앙으로 이동
        playerPositioner.MoveToRoomCenter(room);

        if (room.type == RoomType.Combat && room.state != RoomState.Cleared)
        {
            // 전투방이고 클리어 안 된 상태면 전투 시작
            enemySpawnController.StartCombat(room, roomRenderer);
        }
        else
        {
            // 전투방이 아니거나 이미 클리어된 방은 상태를 클리어로 변경하고 문을 모두 연다
            room.state = RoomState.Cleared;
            roomRenderer.OpenAllDoors(); // 전투방 제외한 방은 즉시 문 열기
        }
    }
}
