using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageSource : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private bool continuousDamage = false;
    [SerializeField] private float tickInterval = 0.5f;

    [Header("Knockback Settings")]
    [SerializeField] private bool applyKnockback = false;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Target Settings")]
    [SerializeField] private LayerMask targetLayers;

    private float nextTickTime;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            Debug.LogWarning("[DamageSource] Collider2D는 IsTrigger로 설정되어야 합니다.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDealDamage(other);

        if (continuousDamage)
            nextTickTime = Time.time + tickInterval;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (continuousDamage && Time.time >= nextTickTime)
        {
            nextTickTime = Time.time + tickInterval;
            TryDealDamage(other);
        }
    }

    private void TryDealDamage(Collider2D other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        IDamageable target = other.GetComponent<IDamageable>();
        if (target == null) return;

        target.TakeDamage(damage);
        Debug.Log($"[DamageSource] {other.name}에게 {damage} 피해 적용");

        if (applyKnockback)
            TryApplyKnockback(other);

        if (destroyOnHit && !continuousDamage)
            Destroy(gameObject);
    }

    private void TryApplyKnockback(Collider2D other)
    {
        Vector2 dir = (other.transform.position - transform.position);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.up;

        dir = dir.normalized;
        Vector2 knockback = dir * knockbackForce;

        var movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ApplyKnockback(knockback);
            Debug.Log($"[DamageSource] PlayerMovement에 넉백 적용: {knockback}");
        }
        else
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(knockback, ForceMode2D.Impulse);
                Debug.Log($"[DamageSource] Rigidbody2D에 넉백 적용: {knockback}");
            }
            else
            {
                Debug.LogWarning("[DamageSource] 넉백 적용 대상에 Rigidbody2D 없음");
            }
        }
    }

    public void SetDamage(int value) => damage = value;
    public void SetTargetLayer(LayerMask layer) => targetLayers = layer;
    public void SetKnockback(float force, bool apply)
    {
        knockbackForce = force;
        applyKnockback = apply;
    }
}
