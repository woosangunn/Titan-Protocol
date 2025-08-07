using UnityEngine;
using MyGame.Map;

/// <summary>
/// �濡 ������ ������ �����ϰ�, ��� ���� óġ�Ǿ��� �� �� ���¸� Cleared�� ��ȯ�ϰ� ���� ��
/// </summary>
public class EnemyTracker : MonoBehaviour
{
    private RoomData room;                    // ���� ������ ���� ���� ���� ������
    private RoomRenderer roomrenderer;        // �� ������ (���� ���� �� ���)
    private int enemyCount = 0;               // ���� �濡 ���� �� ��

    /// <summary>
    /// Ʈ��Ŀ �ʱ�ȭ (�� �����Ϳ� ������ ����)
    /// </summary>
    /// <param name="room">���� ���� ��</param>
    /// <param name="renderer">�� ���⸦ ���� �� ������</param>
    public void Initialize(RoomData room, RoomRenderer renderer)
    {
        this.room = room;
        this.roomrenderer = renderer;
    }

    /// <summary>
    /// ���ο� �� ��� (�� �� ���� �� ��� �̺�Ʈ ����)
    /// </summary>
    /// <param name="enemy">����� �� ���ӿ�����Ʈ</param>
    public void RegisterEnemy(GameObject enemy)
    {
        enemyCount++;

        // ���� �׾��� �� HandleEnemyDeath ȣ���ϵ��� �̺�Ʈ ����
        enemy.GetComponent<EnemyDeathWatcher>().OnDeath += HandleEnemyDeath;
    }

    /// <summary>
    /// ���� �׾��� �� ȣ���. �� �� ���� �� ��� �׾��� ��� �� ���� ó��
    /// </summary>
    private void HandleEnemyDeath()
    {
        enemyCount--;

        // ��� ���� ���ŵǸ� �� ���¸� Cleared�� ��ȯ�ϰ� ���� ����
        if (enemyCount <= 0)
        {
            room.state = RoomState.Cleared;
            roomrenderer.OpenAllDoors();
        }
    }
}
