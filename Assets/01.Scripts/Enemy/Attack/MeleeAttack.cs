using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack/Melee")]
public class MeleeAttack : EnemyAttackBase
{
    private float lastAttackTime;

    public override void Attack(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(controller.transform.position, player.transform.position);
        if (distance <= controller.stats.AttackRange && Time.time >= lastAttackTime + controller.stats.AttackCooldown)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            damageable?.TakeDamage(controller.stats.Damage);
            lastAttackTime = Time.time;
        }
    }
}