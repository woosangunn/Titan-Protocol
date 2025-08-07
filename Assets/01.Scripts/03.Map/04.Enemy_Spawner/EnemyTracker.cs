using UnityEngine;
using MyGame.Map;

/// <summary>
/// 방에 스폰된 적들을 추적하고, 모든 적이 처치되었을 때 방 상태를 Cleared로 전환하고 문을 엶
/// </summary>
public class EnemyTracker : MonoBehaviour
{
    private RoomData room;                    // 현재 전투가 진행 중인 방의 데이터
    private RoomRenderer roomrenderer;        // 방 렌더러 (문을 여는 데 사용)
    private int enemyCount = 0;               // 현재 방에 남은 적 수

    /// <summary>
    /// 트래커 초기화 (방 데이터와 렌더러 연결)
    /// </summary>
    /// <param name="room">적이 속한 방</param>
    /// <param name="renderer">문 열기를 위한 방 렌더러</param>
    public void Initialize(RoomData room, RoomRenderer renderer)
    {
        this.room = room;
        this.roomrenderer = renderer;
    }

    /// <summary>
    /// 새로운 적 등록 (적 수 증가 및 사망 이벤트 연결)
    /// </summary>
    /// <param name="enemy">등록할 적 게임오브젝트</param>
    public void RegisterEnemy(GameObject enemy)
    {
        enemyCount++;

        // 적이 죽었을 때 HandleEnemyDeath 호출하도록 이벤트 연결
        enemy.GetComponent<EnemyDeathWatcher>().OnDeath += HandleEnemyDeath;
    }

    /// <summary>
    /// 적이 죽었을 때 호출됨. 적 수 감소 및 모두 죽었을 경우 문 열기 처리
    /// </summary>
    private void HandleEnemyDeath()
    {
        enemyCount--;

        // 모든 적이 제거되면 방 상태를 Cleared로 전환하고 문을 연다
        if (enemyCount <= 0)
        {
            room.state = RoomState.Cleared;
            roomrenderer.OpenAllDoors();
        }
    }
}
