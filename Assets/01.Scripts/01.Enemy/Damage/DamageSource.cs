using UnityEngine;

/// <summary>
/// 충돌한 대상(IDamageable)에 피해를 주는 범용 트리거
/// - 적 투사체, 함정, 건축물 공격용 등 어디에나 사용 가능
/// - Inspector 또는 코드에서 damage, knockback, 타겟 설정 가능
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageSource : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("피해량")]
    [SerializeField] private int damage = 10;

    [Tooltip("피격 시 투사체 제거 여부")]
    [SerializeField] private bool destroyOnHit = true;

    [Tooltip("지속 피해 여부 (true면 일정 간격으로 피해 적용)")]
    [SerializeField] private bool continuousDamage = false;

    [Tooltip("지속 피해 간격 (초)")]
    [SerializeField] private float tickInterval = 0.5f;

    [Header("Knockback Settings")]
    [Tooltip("넉백 적용 여부")]
    [SerializeField] private bool applyKnockback = false;

    [Tooltip("넉백 세기")]
    [SerializeField] private float knockbackForce = 5f;

    [Header("Target Settings")]
    [Tooltip("피해를 줄 타겟 레이어")]
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

    /// <summary>
    /// 충돌 대상에게 피해와 넉백을 적용
    /// </summary>
    private void TryDealDamage(Collider2D other)
    {
        // 1. 타겟 레이어 필터링
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 2. IDamageable 인터페이스 확인
        IDamageable target = other.GetComponent<IDamageable>();
        if (target == null) return;

        // 3. 데미지 적용
        target.TakeDamage(damage);
        Debug.Log($"[DamageSource] {other.name}에게 {damage} 피해 적용");

        // 4. 넉백 적용
        if (applyKnockback)
            TryApplyKnockback(other);

        // 5. 투사체 제거
        if (destroyOnHit && !continuousDamage)
            Destroy(gameObject);
    }

    /// <summary>
    /// 대상에게 넉백 적용
    /// </summary>
    private void TryApplyKnockback(Collider2D other)
    {
        Vector2 dir = (other.transform.position - transform.position);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.up; // 최소 보정

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

    // -------------------------------
    // 외부에서 설정하기 위한 메서드들
    // -------------------------------

    /// <summary>피해량 설정</summary>
    public void SetDamage(int value) => damage = value;

    /// <summary>타겟 레이어 설정</summary>
    public void SetTargetLayer(LayerMask layer) => targetLayers = layer;

    /// <summary>넉백 세기 및 적용 여부 설정</summary>
    public void SetKnockback(float force, bool apply)
    {
        knockbackForce = force;
        applyKnockback = apply;
    }
}
