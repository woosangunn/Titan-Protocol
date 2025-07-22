using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/Attack/Ranged Flexible")]
public class RangedAttack : EnemyAttackBase
{
    [Header("Projectile Settings")]
    [Tooltip("�߻�ü ������")]
    public GameObject projectilePrefab;

    [Tooltip("�߻�ü �ӵ�")]
    public float projectileSpeed = 5f;

    [Tooltip("���� ���ط�")]
    public int damage = 10;

    [Header("Multi-shot Settings")]
    [Tooltip("�߻��� �Ѿ� ��")]
    public int bulletCount = 1;

    [Tooltip("�Ѿ� ���� ���� (��ä�� �߻�)")]
    public float spreadAngle = 0f;

    [Tooltip("���� ���� (��, 0�̸� ���� �߻�)")]
    public float fireInterval = 0f;

    /// <summary>
    /// ���� ���� ����
    /// - ���� ��ٿ� üũ �� �÷��̾� ��ġ ���
    /// - ���� Ȥ�� ���� �߻� ����
    /// - ��ä�� ��ź ����
    /// </summary>
    /// <param name="controller">�� ��Ʈ�ѷ�</param>
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
            // ���� �߻� (��ä�� ��ź ����)
            for (int i = 0; i < bulletCount; i++)
            {
                float angleOffset = (bulletCount == 1) ? 0f :
                    spreadAngle * ((i / (float)(bulletCount - 1)) - 0.5f);

                Fire(controller, baseAngle + angleOffset);
            }
        }
        else
        {
            // ���� �߻� (�ڷ�ƾ ���)
            controller.StartCoroutine(FireBurst(controller, baseAngle));
        }
    }

    /// <summary>
    /// ���� �ڷ�ƾ
    /// </summary>
    /// <param name="controller">�� ��Ʈ�ѷ�</param>
    /// <param name="baseAngle">���� ����</param>
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
    /// ���� �Ѿ� �߻�
    /// - ���� ����, �ӵ� �ο�
    /// - ���ط� �� Ÿ�� ���̾� ����
    /// </summary>
    /// <param name="controller">�� ��Ʈ�ѷ�</param>
    /// <param name="angle">�߻� ���� (�� ����)</param>
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
