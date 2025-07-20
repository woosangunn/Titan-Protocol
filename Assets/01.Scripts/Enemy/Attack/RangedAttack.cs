using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/Attack/Ranged Flexible")]
public class RangedAttack : EnemyAttackBase
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public int damage = 10;

    [Header("Multi-shot Settings")]
    public int bulletCount = 1;            // 몇 발 쏘는가?
    public float spreadAngle = 0f;         // 총 퍼짐 각도
    public float fireInterval = 0f;        // 연사 간격 (0이면 동시 발사)

    public override void Attack(EnemyController controller)
    {
        if (Time.time < controller.LastAttackTime + controller.Stats.AttackCooldown)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 baseDir = (player.transform.position - controller.transform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        controller.LastAttackTime = Time.time;

        if (fireInterval <= 0f || bulletCount == 1)
        {
            // 동시 발사 (부채꼴 산탄 포함)
            for (int i = 0; i < bulletCount; i++)
            {
                float angleOffset = (bulletCount == 1) ? 0f :
                    spreadAngle * ((i / (float)(bulletCount - 1)) - 0.5f);

                Fire(controller, baseAngle + angleOffset);
            }
        }
        else
        {
            // 연사 발사 (코루틴 활용)
            controller.StartCoroutine(FireBurst(controller, baseAngle));
        }
    }

    private IEnumerator FireBurst(EnemyController controller, float baseAngle)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Fire(controller, baseAngle);
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void Fire(EnemyController controller, float angle)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        Bullet bullet = BulletPool.Instance.GetBullet(
            BulletType.Enemy,
            controller.transform.position,
            Quaternion.Euler(0, 0, angle)
        );

        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;

        var ds = bullet.GetComponent<DamageSource>();
        if (ds != null)
        {
            ds.SetDamage(damage);
            ds.SetTargetLayer(LayerMask.GetMask("Player"));
        }
    }
}
