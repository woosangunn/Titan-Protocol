using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/Attack/Ranged Flexible")]
public class RangedAttack : EnemyAttackBase
{
    [Header("Projectile Settings")]
    [Tooltip("발사체 프리팹")]
    public GameObject projectilePrefab;

    [Tooltip("발사체 속도")]
    public float projectileSpeed = 5f;

    [Tooltip("공격 피해량")]
    public int damage = 10;

    [Header("Multi-shot Settings")]
    [Tooltip("발사할 총알 수")]
    public int bulletCount = 1;

    [Tooltip("총알 퍼짐 각도 (부채꼴 발사)")]
    public float spreadAngle = 0f;

    [Tooltip("연사 간격 (초, 0이면 동시 발사)")]
    public float fireInterval = 0f;

    /// <summary>
    /// 적의 공격 수행
    /// - 공격 쿨다운 체크 후 플레이어 위치 계산
    /// - 연사 혹은 동시 발사 결정
    /// - 부채꼴 산탄 포함
    /// </summary>
    /// <param name="controller">적 컨트롤러</param>
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
            // 연사 발사 (코루틴 사용)
            controller.StartCoroutine(FireBurst(controller, baseAngle));
        }
    }

    /// <summary>
    /// 연사 코루틴
    /// </summary>
    /// <param name="controller">적 컨트롤러</param>
    /// <param name="baseAngle">기준 각도</param>
    /// <returns></returns>
    private IEnumerator FireBurst(EnemyController controller, float baseAngle)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Fire(controller, baseAngle);
            yield return new WaitForSeconds(fireInterval);
        }
    }

    /// <summary>
    /// 단일 총알 발사
    /// - 방향 설정, 속도 부여
    /// - 피해량 및 타겟 레이어 지정
    /// </summary>
    /// <param name="controller">적 컨트롤러</param>
    /// <param name="angle">발사 각도 (도 단위)</param>
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
