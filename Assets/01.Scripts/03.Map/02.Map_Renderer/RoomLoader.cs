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
        if (roomManager.GetRoomAt(next) == null)
        {
            Debug.LogWarning($"이동할 수 없는 방향: {dir}");
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
        tileRenderer.DrawRoom(room, origin);  // tileSize 기반으로 그려야 함

        // 변경된 부분: tileSize 기준으로 중앙 위치 계산
        Vector2Int tileSize = room.tileSize;
        player.position = origin + new Vector3(tileSize.x, tileSize.y) * 0.5f;

        SpawnEnemiesAndTrack(room);
    }

    private void SpawnEnemiesAndTrack(RoomData room)
    {
        GameObject trackerObj = new GameObject("EnemyTracker");
        EnemyTracker tracker = trackerObj.AddComponent<EnemyTracker>();
        tracker.OnAllEnemiesDefeated += () => tileRenderer.OpenAllDoors();
        tracker.ForceClear(); // 테스트용
    }
}
