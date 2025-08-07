using UnityEngine;
using MyGame.Map;

/// <summary>
/// �� ���� ���� ������Ʈ. �� �� ������ ��ġ�� �� �������� �����ϰ�, EnemyTracker�� ���.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // ������ �� ������

    /// <summary>
    /// �� �� �� ���� ��ġ���� ��ȸ�ϸ� �� ���� �� Ʈ��Ŀ�� ���
    /// </summary>
    /// <param name="room">���� ��ġ ������ ��� �� ������</param>
    /// <param name="tracker">������ ���� ������ Ʈ��Ŀ</param>
    public void SpawnEnemies(RoomData room, EnemyTracker tracker)
    {
        foreach (var spawnPoint in room.enemySpawnPoints)
        {
            // ���� ������ ���� ��ǥ�� ��ȯ
            Vector3 worldPos = new Vector3(spawnPoint.x, spawnPoint.y, 0);

            // �� ������ ����
            GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            // Ʈ��Ŀ�� �� ��� (��� ���� ��)
            tracker.RegisterEnemy(enemy);
        }
    }
}
