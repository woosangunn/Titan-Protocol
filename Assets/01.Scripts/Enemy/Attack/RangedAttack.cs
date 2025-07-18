using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack/Ranged")]
public class RangedAttack : EnemyAttackBase
{
    public GameObject projectilePrefab;
    private float lastAttackTime;

    public override void Attack(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || projectilePrefab == null) return;

        float distance = Vector2.Distance(controller.transform.position, player.transform.position);
        if (distance <= controller.stats.AttackRange && Time.time >= lastAttackTime + controller.stats.AttackCooldown)
        {
            GameObject proj = Instantiate(projectilePrefab, controller.transform.position, Quaternion.identity);
            Vector2 dir = (player.transform.position - controller.transform.position).normalized;
            proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 5f;
            lastAttackTime = Time.time;
        }
    }
}