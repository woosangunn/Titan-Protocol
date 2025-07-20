using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Movement/Kite Player")]
public class KitePlayerMovement : EnemyMovementBase
{
    public float minDistance = 4f;
    public float maxDistance = 6f;

    public override void Move(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 dirToPlayer = player.transform.position - controller.transform.position;
        float distance = dirToPlayer.magnitude;
        Vector2 direction = dirToPlayer.normalized;

        if (distance < minDistance)
        {
            // 플레이어와 너무 가까움 → 후퇴
            controller.transform.position -= (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
        else if (distance > maxDistance)
        {
            // 너무 멀다 → 접근
            controller.transform.position += (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
        // 사거리 안이면 정지
    }
}
