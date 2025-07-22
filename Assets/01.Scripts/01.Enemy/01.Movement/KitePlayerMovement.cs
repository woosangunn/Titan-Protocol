using UnityEngine;

/// <summary>
/// 플레이어를 일정 거리 유지하며 후퇴하거나 접근하는 적의 이동 패턴
/// - 최소 거리 이내면 후퇴, 최대 거리 이상이면 접근
/// - 일정 사거리 내에 들어오면 정지
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Movement/Kite Player")]
public class KitePlayerMovement : EnemyMovementBase
{
    /// <summary>
    /// 플레이어와 최소 유지 거리 (이 거리보다 가까우면 후퇴)
    /// </summary>
    public float minDistance = 4f;

    /// <summary>
    /// 플레이어와 최대 유지 거리 (이 거리보다 멀면 접근)
    /// </summary>
    public float maxDistance = 6f;

    /// <summary>
    /// 매 프레임 호출되어 플레이어와의 거리를 체크하고 후퇴 또는 접근 이동 수행
    /// </summary>
    /// <param name="controller">이동을 수행할 EnemyController 인스턴스</param>
    public override void Move(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 dirToPlayer = player.transform.position - controller.transform.position;
        float distance = dirToPlayer.magnitude;
        Vector2 direction = dirToPlayer.normalized;

        if (distance < minDistance)
        {
            // 플레이어와 너무 가까우면 후퇴
            controller.transform.position -= (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
        else if (distance > maxDistance)
        {
            // 플레이어와 너무 멀면 접근
            controller.transform.position += (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
        // minDistance 이상 maxDistance 이하일 때는 정지 (이동 없음)
    }
}
