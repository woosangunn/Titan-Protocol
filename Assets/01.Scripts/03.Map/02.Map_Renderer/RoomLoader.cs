using UnityEngine;
using MyGame.Map;

/// <summary>
/// �÷��̾� ��ġ�� ���� ���� �ε��ϰ�, �� ���� �� �� ��� ���
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
            Debug.LogWarning($"�̵��� �� ���� ����: {dir}");
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
            Debug.LogError("��ȿ���� ���� �� ��ǥ: " + roomCoord);
            return;
        }

        tileRenderer.ClearTiles();

        Vector3Int origin = Vector3Int.zero;
        tileRenderer.DrawRoom(room, origin);  // tileSize ������� �׷��� ��

        // ����� �κ�: tileSize �������� �߾� ��ġ ���
        Vector2Int tileSize = room.tileSize;
        player.position = origin + new Vector3(tileSize.x, tileSize.y) * 0.5f;

        SpawnEnemiesAndTrack(room);
    }

    private void SpawnEnemiesAndTrack(RoomData room)
    {
        GameObject trackerObj = new GameObject("EnemyTracker");
        EnemyTracker tracker = trackerObj.AddComponent<EnemyTracker>();
        tracker.OnAllEnemiesDefeated += () => tileRenderer.OpenAllDoors();
        tracker.ForceClear(); // �׽�Ʈ��
    }
}
