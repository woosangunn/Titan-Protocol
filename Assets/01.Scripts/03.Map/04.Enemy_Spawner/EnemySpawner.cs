using UnityEngine;
using MyGame.Map;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // ������ �� ������

    public void SpawnEnemies(RoomData room, EnemyTracker tracker)
    {
        foreach (var spawnPoint in room.enemySpawnPoints)
        {
            Vector3 worldPos = new Vector3(spawnPoint.x, spawnPoint.y, 0);

            GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            tracker.RegisterEnemy(enemy);
        }
    }
}
