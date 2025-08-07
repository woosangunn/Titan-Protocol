using UnityEngine;
using MyGame.Map;

/// <summary>
/// �÷��̾ Ư�� ���� �߾� ��ġ�� �̵���Ű�� ������ �����ϴ� Ŭ����
/// </summary>
public class PlayerPositioner : MonoBehaviour
{
    public Transform player;  // �̵���ų �÷��̾� Transform ����

    /// <summary>
    /// ���޹��� ��(RoomData)�� �߾� ��ġ�� �÷��̾� ��ġ�� �̵�
    /// </summary>
    /// <param name="room">�̵��� ��� �� ������</param>
    public void MoveToRoomCenter(RoomData room)
    {
        // �� Ÿ���� �߾� ���� ��ǥ�� ������ �÷��̾� ��ġ�� ����
        player.position = room.GetTileCenterWorldPos();
    }
}
