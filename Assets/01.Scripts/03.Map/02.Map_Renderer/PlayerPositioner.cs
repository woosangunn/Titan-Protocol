using MyGame.Map;
using UnityEngine;

public class PlayerPositioner : MonoBehaviour
{
    public Transform player;

    public void MoveToRoomCenter(RoomData room)
    {
        player.position = room.GetTileCenterWorldPos();
    }

    // 방에 doorDirection 방향 문 위치가 있으면 플레이어를 그 앞에 이동시키고 true 반환
    // 없으면 false 반환
    public bool MoveToDoorFront(RoomData room, Vector2Int doorDirection, Vector2Int lastMoveDir)
    {
        Vector2Int? doorPos = room.GetDoorTilePosition(doorDirection);
        if (doorPos.HasValue)
        {
            Vector3Int tileCellPos = new Vector3Int(doorPos.Value.x, doorPos.Value.y, 0);
            Vector3 baseWorldPos = new Vector3(tileCellPos.x, tileCellPos.y, 0f);

            Vector3 offset = Vector3.zero;

            // 이동한 방향(lastMoveDir)에 따라 오프셋 직접 지정
            if (lastMoveDir == Vector2Int.right)
                offset = new Vector3(2f, 0, 0);     // 오른쪽으로 들어왔으면 +X 방향으로 1 유닛 이동
            else if (lastMoveDir == Vector2Int.left)
                offset = new Vector3(-2f, 0, 0);    // 왼쪽으로 들어왔으면 -X 방향으로 1 유닛 이동
            else if (lastMoveDir == Vector2Int.up)
                offset = new Vector3(0, 2f, 0);     // 위쪽으로 들어왔으면 +Y 방향으로 1 유닛 이동
            else if (lastMoveDir == Vector2Int.down)
                offset = new Vector3(0, -2f, 0);    // 아래쪽으로 들어왔으면 -Y 방향으로 1 유닛 이동

            player.position = baseWorldPos + offset;

            return true;
        }
        return false;
    }
}
