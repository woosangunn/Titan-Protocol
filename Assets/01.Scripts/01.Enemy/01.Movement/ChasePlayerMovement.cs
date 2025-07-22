using UnityEngine;

/// <summary>
/// 플레이어를 쫓아가는 적의 이동 패턴
/// - 플레이어 방향으로 이동
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Movement/Chase Player")]
public class ChasePlayerMovement : EnemyMovementBase
{
    /// <summary>
    /// 매 프레임 호출되어 플레이어 방향으로 이동 처리
    /// </summary>
    /// <param name="controller">이동을 수행할 EnemyController 인스턴스</param>
    public override void Move(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 direction = (player.transform.position - controller.transform.position).normalized;
        controller.transform.position += (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
    }
}
