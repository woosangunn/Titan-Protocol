using UnityEngine;
using MyGame.Map;

public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemyPrefab;

    private EnemySpawner spawner;

    private void Awake()
    {
        spawner = gameObject.AddComponent<EnemySpawner>();
        spawner.enemyPrefab = enemyPrefab;
    }

    public void StartCombat(RoomData room, RoomRenderer renderer)
    {
        GameObject trackerObj = new GameObject("EnemyTracker");

        EnemyTracker tracker = trackerObj.AddComponent<EnemyTracker>();
        tracker.Initialize(room, renderer);

        spawner.SpawnEnemies(room, tracker);
    }
}
