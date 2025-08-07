using UnityEngine;
using MyGame.Map;

/// <summary>
/// ���� ����, �� ���� �� �� ����(Ʈ��Ŀ) ������ �����ϴ� Ŭ����
/// </summary>
public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemyPrefab; // ������ �� ������ ����

    private EnemySpawner spawner;  // ���� �� ���� ��� ������Ʈ

    private void Awake()
    {
        // �� ���� ������Ʈ�� EnemySpawner ������Ʈ �߰� �� ������ ����
        spawner = gameObject.AddComponent<EnemySpawner>();
        spawner.enemyPrefab = enemyPrefab;
    }

    /// <summary>
    /// ������ �����ϰ�, �� ������ �� ������ ���� Ʈ��Ŀ �ʱ�ȭ
    /// </summary>
    /// <param name="room">���� ������ �� ������</param>
    /// <param name="renderer">�� ������ (�� ��ġ ��� � ���)</param>
    public void StartCombat(RoomData room, RoomRenderer renderer)
    {
        // �� ������ �� ���ӿ�����Ʈ ����
        GameObject trackerObj = new GameObject("EnemyTracker");

        // EnemyTracker ������Ʈ �߰� �� �ʱ�ȭ
        EnemyTracker tracker = trackerObj.AddComponent<EnemyTracker>();
        tracker.Initialize(room, renderer);

        // EnemySpawner�� ���� ���� �����ϸ� Ʈ��Ŀ�� ����
        spawner.SpawnEnemies(room, tracker);
    }
}
