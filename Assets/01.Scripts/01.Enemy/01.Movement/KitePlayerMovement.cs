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
            controller.transform.position -= (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
        else if (distance > maxDistance)
        {
            controller.transform.position += (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
    }
}
