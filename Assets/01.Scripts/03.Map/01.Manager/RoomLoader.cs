using UnityEngine;
using MyGame.Map;

public class RoomLoader : MonoBehaviour
{
    public RoomManager roomManager;
    public RoomRenderer roomRenderer;
    public PlayerPositioner playerPositioner;
    public EnemySpawnController enemySpawnController;

    private Vector2Int currentRoomCoord = Vector2Int.zero;

    private Vector2Int lastMoveDir = Vector2Int.zero;

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

        lastMoveDir = dir;
        currentRoomCoord = next;
        LoadRoom(currentRoomCoord);
    }

    private void LoadRoom(Vector2Int roomCoord)
    {
        RoomData room = roomManager.GetRoomAt(roomCoord);
        if (room == null) return;

        if (room.state == RoomState.Unvisited)
            room.state = RoomState.Discovered;

        roomRenderer.Render(room, Vector3Int.zero, this);

        if (lastMoveDir == Vector2Int.zero)
        {
            // 초기 시작 방일 때는 그냥 방 중앙으로 이동
            playerPositioner.MoveToRoomCenter(room);
        }
        else
        {
            Vector2Int oppositeDir = new Vector2Int(-lastMoveDir.x, -lastMoveDir.y);
            bool movedToDoor = playerPositioner.MoveToDoorFront(room, oppositeDir, lastMoveDir);

            if (!movedToDoor)
                playerPositioner.MoveToRoomCenter(room);
        }

        if (room.type == RoomType.Combat && room.state != RoomState.Cleared)
        {
            enemySpawnController.StartCombat(room, roomRenderer);
        }
        else
        {
            room.state = RoomState.Cleared;
            roomRenderer.OpenAllDoors();
        }
    }
}
