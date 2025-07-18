using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Movement/Chase Player")]
public class ChasePlayerMovement : EnemyMovementBase
{
    public override void Move(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 direction = (player.transform.position - controller.transform.position).normalized;
        controller.transform.position += (Vector3)(direction * controller.stats.MoveSpeed * Time.deltaTime);
    }
}
