using UnityEngine;

/// <summary>
/// �浹�� ���(IDamageable)�� ���ظ� �ִ� ���� Ʈ����
/// - �� ����ü, ����, ���๰ ���ݿ� �� ��𿡳� ��� ����
/// - Inspector���� ���ݷ�, �˹�, ��� ���̾� ���� �ս��� ����
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageSource : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;                    // ���ݷ�
    public bool destroyOnHit = true;           // �ǰ� �� �ڸ� ����
    public bool continuousDamage = false;      // Ʈ���� �ȿ� �ӹ����� ���� ���� ����?
    public float tickInterval = 0.5f;          // ���� ���� �ֱ�

    [Header("Knockback")]
    public bool applyKnockback = false;        // �˹� ���� ����
    public float knockbackForce = 5f;          // �˹� ��

    [Header("Target Filter")]
    public LayerMask targetLayers;             // �� ���̾ ���� ������Ʈ�� ����

    private float nextTickTime;                // ���� ���ؿ� Ÿ�̸�

    void Awake()
    {
        // Collider2D�� Trigger ������� Ȯ��
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            Debug.LogWarning("[DamageSource] Collider2D�� IsTrigger = true �� �����ϼ���.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDealDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (continuousDamage && Time.time >= nextTickTime)
        {
            nextTickTime = Time.time + tickInterval;
            TryDealDamage(other);
        }
    }

    /// <summary>
    /// ����� ���ظ� ���� �� ������ ���������˹� ó��
    /// </summary>
    private void TryDealDamage(Collider2D other)
    {
        // 1) ���̾� ���͸�
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 2) IDamageable �������̽� Ȯ��
        IDamageable dmgTarget = other.GetComponent<IDamageable>();
        if (dmgTarget == null) return;

        // 3) ������ ����
        dmgTarget.TakeDamage(damage);

        // 4) �˹� ����(����)
        if (applyKnockback)
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }

        // 5) ����ü ���� 1ȸ ��Ʈ �� �ı�
        if (destroyOnHit && !continuousDamage)
            Destroy(gameObject);
    }
}
