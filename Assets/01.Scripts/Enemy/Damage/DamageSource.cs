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

        // ���� ���ؿ� Ÿ�̸� �ʱ�ȭ
        if (continuousDamage)
            nextTickTime = Time.time + tickInterval;
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
        Debug.Log($"[DamageSource] {other.name}���� {damage} ������ ����");

        // 4) �˹� ����(����)
        if (applyKnockback)
        {
            Vector2 dir = (other.transform.position - transform.position);
            if (dir.sqrMagnitude < 0.01f)
                dir = Vector2.up; // �ּ� ����

            dir = dir.normalized;
            Vector2 knockback = dir * knockbackForce;

            // PlayerMovement �� �˹� ó�� �켱
            var movement = other.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.ApplyKnockback(knockback);
                Debug.Log($"[DamageSource] {other.name}�� ApplyKnockback ȣ��: ���� {dir}, �� {knockbackForce}");
            }
            else
            {
                // ������ Rigidbody2D ���� ó��
                Rigidbody2D rb = other.attachedRigidbody;
                if (rb != null)
                {
                    rb.AddForce(knockback, ForceMode2D.Impulse);
                    Debug.Log($"[DamageSource] {other.name}�� Rigidbody2D�� �˹� ����");
                }
                else
                {
                    Debug.LogWarning($"[DamageSource] {other.name}�� Rigidbody2D ���� (�˹� �Ұ�)");
                }
            }
        }

        // 5) ����ü ���� 1ȸ ��Ʈ �� �ı�
        if (destroyOnHit && !continuousDamage)
            Destroy(gameObject);
    }
}
