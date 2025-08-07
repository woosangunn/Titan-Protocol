using UnityEngine;
using MyGame.Map;

/// <summary>
/// �� �̵� �� �� ������ �ε��� �����ϴ� ��Ʈ�ѷ� Ŭ����
/// �� �̵� �� �� ������ �ҷ�����, �÷��̾� ��ġ �� �� ���� ���� ������
/// </summary>
public class RoomLoader : MonoBehaviour
{
    [Header("References")]
    public RoomManager roomManager;                // �� �� ��� ���� �����ϴ� �Ŵ���
    public RoomRenderer roomRenderer;              // ���� ȭ�鿡 �׸��� ������
    public PlayerPositioner playerPositioner;      // �÷��̾� ��ġ�� �� �߽����� �̵���Ű�� ����
    public EnemySpawnController enemySpawnController;  // ���� ���� �� �� ���� ���

    [Header("���� �� ��ġ")]
    private Vector2Int currentRoomCoord = Vector2Int.zero;  // ���� �÷��̾ �ִ� �� ��ǥ (�� ����)

    private void Start()
    {
        // ���� ���� �� ó�� �� �ε�
        LoadRoom(currentRoomCoord);
    }

    /// <summary>
    /// �÷��̾ Ư�� �������� �� �̵��� �õ��� �� ȣ��
    /// </summary>
    /// <param name="dir">�̵��� ���� ���� (up/down/left/right)</param>
    public void TryMove(Vector2Int dir)
    {
        Vector2Int next = currentRoomCoord + dir;

        // �̵��Ϸ��� ���� �������� ������ �̵� �Ұ� ó��
        if (roomManager.GetRoomAt(next) == null)
        {
            Debug.LogWarning($"�̵��� �� ���� ����: {dir}");
            return;
        }

        // ���� �� ��ǥ ���� �� �ش� �� �ε�
        currentRoomCoord = next;
        LoadRoom(currentRoomCoord);
    }

    /// <summary>
    /// Ư�� ��ǥ�� �� �����͸� �ҷ��� ������ �� ���¿� ���� �� ����, �� ���� ó��
    /// </summary>
    /// <param name="roomCoord">�ҷ��� ���� ��ǥ</param>
    private void LoadRoom(Vector2Int roomCoord)
    {
        RoomData room = roomManager.GetRoomAt(roomCoord);
        if (room == null) return;

        // ó�� �湮�ϴ� ���̸� ���¸� �߰�(Discovered)���� ����
        if (room.state == RoomState.Unvisited)
            room.state = RoomState.Discovered;

        // �� ������, ��ǥ �������� Vector3Int.zero (���� ����)
        roomRenderer.Render(room, Vector3Int.zero, this);

        // �÷��̾ �� �߾����� �̵�
        playerPositioner.MoveToRoomCenter(room);

        if (room.type == RoomType.Combat && room.state != RoomState.Cleared)
        {
            // �������̰� Ŭ���� �� �� ���¸� ���� ����
            enemySpawnController.StartCombat(room, roomRenderer);
        }
        else
        {
            // �������� �ƴϰų� �̹� Ŭ����� ���� ���¸� Ŭ����� �����ϰ� ���� ��� ����
            room.state = RoomState.Cleared;
            roomRenderer.OpenAllDoors(); // ������ ������ ���� ��� �� ����
        }
    }
}
