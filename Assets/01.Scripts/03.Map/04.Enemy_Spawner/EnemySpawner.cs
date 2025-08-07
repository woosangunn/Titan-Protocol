using UnityEngine;
using MyGame.Map;

/// <summary>
/// 적 생성 전담 컴포넌트. 방 내 지정된 위치에 적 프리팹을 스폰하고, EnemyTracker에 등록.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // 생성할 적 프리팹

    /// <summary>
    /// 방 내 적 스폰 위치들을 순회하며 적 생성 및 트래커에 등록
    /// </summary>
    /// <param name="room">스폰 위치 정보가 담긴 방 데이터</param>
    /// <param name="tracker">생성된 적을 관리할 트래커</param>
    public void SpawnEnemies(RoomData room, EnemyTracker tracker)
    {
        foreach (var spawnPoint in room.enemySpawnPoints)
        {
            // 스폰 지점을 월드 좌표로 변환
            Vector3 worldPos = new Vector3(spawnPoint.x, spawnPoint.y, 0);

            // 적 프리팹 생성
            GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            // 트래커에 적 등록 (사망 감시 등)
            tracker.RegisterEnemy(enemy);
        }
    }
}
