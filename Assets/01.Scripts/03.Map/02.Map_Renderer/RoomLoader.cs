using UnityEngine;
using MyGame.Map;

/// <summary>
/// 플레이어 위치에 따라 방을 로드하고, 적 추적 및 문 제어를 담당
/// </summary>
public class RoomLoader : MonoBehaviour
{
    public RoomManager roomManager;
    public RoomTileRenderer tileRenderer;
    public Transform player;

    private Vector2Int currentRoomCoord = Vector2Int.zero;

    private void Start()
    {
        LoadRoom(currentRoomCoord);
    }

    public void TryMove(Vector2Int dir)
    {
        Vector2Int next = currentRoomCoord + dir;

        Debug.Log($"[RoomLoader] TryMove 호출: 현재={currentRoomCoord}, 방향={dir}, 다음={next}");

        if (roomManager.GetRoomAt(next) == null)
        {
            Debug.LogWarning($"이동 불가: 방이 없음 {next}");
            return;
        }

        currentRoomCoord = next;
        LoadRoom(currentRoomCoord);
    }

    private void LoadRoom(Vector2Int roomCoord)
    {
        RoomData room = roomManager.GetRoomAt(roomCoord);
        if (room == null)
        {
            Debug.LogError("유효하지 않은 방 좌표: " + roomCoord);
            return;
        }

        tileRenderer.ClearTiles();

        Vector3Int origin = Vector3Int.zero;
        tileRenderer.DrawRoom(room, origin);

        Vector2Int size = room.size;
        player.position = origin + new Vector3(size.x, size.y) * 0.5f;

        Debug.Log($"[RoomLoader] {room.position} 방 로드 완료. 문 수: {room.doorLocalPositions.Count}");

        SpawnEnemiesAndTrack(room);
    }

    private void SpawnEnemiesAndTrack(RoomData room)
    {
        GameObject trackerObj = new GameObject("EnemyTracker");
        EnemyTracker tracker = trackerObj.AddComponent<EnemyTracker>();

        tracker.OnAllEnemiesDefeated += () =>
        {
            tileRenderer.OpenAllDoors();
            Debug.Log("방 클리어됨 - 문 열림");
        };

        tracker.ForceClear(); // 테스트 중 즉시 클리어
    }
}
