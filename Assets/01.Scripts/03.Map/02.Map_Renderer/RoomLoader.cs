using UnityEngine;
using MyGame.Map;

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

        RenderRoom(room);
        PositionPlayer(room);
        SpawnEnemiesAndTrack(room);
    }

    private void RenderRoom(RoomData room)
    {
        tileRenderer.ClearTiles();
        tileRenderer.DrawRoom(room, Vector3Int.zero);
    }

    private void PositionPlayer(RoomData room)
    {
        player.position = room.GetTileCenterWorldPos();
    }

    private void SpawnEnemiesAndTrack(RoomData room)
    {
        GameObject trackerObj = new GameObject("EnemyTracker");
        EnemyTracker tracker = trackerObj.AddComponent<EnemyTracker>();
        tracker.OnAllEnemiesDefeated += () => tileRenderer.OpenAllDoors();
        tracker.ForceClear(); // �׽�Ʈ��
    }
}
