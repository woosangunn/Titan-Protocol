using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack/Suicide")]
public class SuicideAttack : EnemyAttackBase
{
    public override void Attack(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(controller.transform.position, player.transform.position);
        if (distance <= controller.Stats.AttackRange)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            damageable?.TakeDamage(controller.Stats.Damage * 3);
            Object.Destroy(controller.gameObject);
        }
    }
}