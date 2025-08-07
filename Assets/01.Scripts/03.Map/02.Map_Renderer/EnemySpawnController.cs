using UnityEngine;
using MyGame.Map;

/// <summary>
/// 전투 시작, 적 스폰 및 적 추적(트래커) 관리를 전담하는 클래스
/// </summary>
public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemyPrefab; // 생성할 적 프리팹 참조

    private EnemySpawner spawner;  // 실제 적 생성 담당 컴포넌트

    private void Awake()
    {
        // 이 게임 오브젝트에 EnemySpawner 컴포넌트 추가 및 프리팹 설정
        spawner = gameObject.AddComponent<EnemySpawner>();
        spawner.enemyPrefab = enemyPrefab;
    }

    /// <summary>
    /// 전투를 시작하고, 적 생성과 적 추적을 위한 트래커 초기화
    /// </summary>
    /// <param name="room">적이 생성될 방 데이터</param>
    /// <param name="renderer">방 렌더러 (적 위치 계산 등에 사용)</param>
    public void StartCombat(RoomData room, RoomRenderer renderer)
    {
        // 적 추적용 빈 게임오브젝트 생성
        GameObject trackerObj = new GameObject("EnemyTracker");

        // EnemyTracker 컴포넌트 추가 후 초기화
        EnemyTracker tracker = trackerObj.AddComponent<EnemyTracker>();
        tracker.Initialize(room, renderer);

        // EnemySpawner를 통해 적을 스폰하며 트래커와 연결
        spawner.SpawnEnemies(room, tracker);
    }
}
