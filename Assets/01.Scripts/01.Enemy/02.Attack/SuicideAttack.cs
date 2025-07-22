using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack/Suicide")]
public class SuicideAttack : EnemyAttackBase
{
    /// <summary>
    /// 적이 플레이어와 일정 거리 내에 있을 때 즉시 피해를 입히고 자신을 파괴하는 공격
    /// </summary>
    /// <param name="controller">적 컨트롤러</param>
    public override void Attack(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(controller.transform.position, player.transform.position);
        if (distance <= controller.Stats.AttackRange)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            damageable?.TakeDamage(controller.Stats.Damage * 3); // 3배 피해

            // 공격 후 자신(적) 즉시 파괴
            Object.Destroy(controller.gameObject);
        }
    }
}
