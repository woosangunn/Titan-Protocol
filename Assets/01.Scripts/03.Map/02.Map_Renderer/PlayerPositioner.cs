using MyGame.Map;
using UnityEngine;

public class PlayerPositioner : MonoBehaviour
{
    public Transform player;

    public void MoveToRoomCenter(RoomData room)
    {
        player.position = room.GetTileCenterWorldPos();
    }

    // �濡 doorDirection ���� �� ��ġ�� ������ �÷��̾ �� �տ� �̵���Ű�� true ��ȯ
    // ������ false ��ȯ
    public bool MoveToDoorFront(RoomData room, Vector2Int doorDirection, Vector2Int lastMoveDir)
    {
        Vector2Int? doorPos = room.GetDoorTilePosition(doorDirection);
        if (doorPos.HasValue)
        {
            Vector3Int tileCellPos = new Vector3Int(doorPos.Value.x, doorPos.Value.y, 0);
            Vector3 baseWorldPos = new Vector3(tileCellPos.x, tileCellPos.y, 0f);

            Vector3 offset = Vector3.zero;

            // �̵��� ����(lastMoveDir)�� ���� ������ ���� ����
            if (lastMoveDir == Vector2Int.right)
                offset = new Vector3(2f, 0, 0);     // ���������� �������� +X �������� 1 ���� �̵�
            else if (lastMoveDir == Vector2Int.left)
                offset = new Vector3(-2f, 0, 0);    // �������� �������� -X �������� 1 ���� �̵�
            else if (lastMoveDir == Vector2Int.up)
                offset = new Vector3(0, 2f, 0);     // �������� �������� +Y �������� 1 ���� �̵�
            else if (lastMoveDir == Vector2Int.down)
                offset = new Vector3(0, -2f, 0);    // �Ʒ������� �������� -Y �������� 1 ���� �̵�

            player.position = baseWorldPos + offset;

            return true;
        }
        return false;
    }
}
