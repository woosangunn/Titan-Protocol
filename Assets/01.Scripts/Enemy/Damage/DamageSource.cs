using UnityEngine;

/// <summary>
/// 충돌한 대상(IDamageable)에 피해를 주는 범용 트리거
/// - 적 투사체, 함정, 건축물 공격용 등 어디에나 사용 가능
/// - Inspector에서 공격력, 넉백, 대상 레이어 등을 손쉽게 설정
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageSource : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;                    // 공격력
    public bool destroyOnHit = true;           // 피격 후 자멸 여부
    public bool continuousDamage = false;      // 트리거 안에 머무르는 동안 지속 피해?
    public float tickInterval = 0.5f;          // 지속 피해 주기

    [Header("Knockback")]
    public bool applyKnockback = false;        // 넉백 적용 여부
    public float knockbackForce = 5f;          // 넉백 힘

    [Header("Target Filter")]
    public LayerMask targetLayers;             // 이 레이어에 속한 오브젝트만 피해

    private float nextTickTime;                // 지속 피해용 타이머

    void Awake()
    {
        // Collider2D가 Trigger 모드인지 확인
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            Debug.LogWarning("[DamageSource] Collider2D를 IsTrigger = true 로 설정하세요.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDealDamage(other);

        // 지속 피해용 타이머 초기화
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
    /// 대상이 피해를 받을 수 있으면 데미지·넉백 처리
    /// </summary>
    private void TryDealDamage(Collider2D other)
    {
        // 1) 레이어 필터링
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 2) IDamageable 인터페이스 확인
        IDamageable dmgTarget = other.GetComponent<IDamageable>();
        if (dmgTarget == null) return;

        // 3) 데미지 적용
        dmgTarget.TakeDamage(damage);
        Debug.Log($"[DamageSource] {other.name}에게 {damage} 데미지 적용");

        // 4) 넉백 적용(선택)
        if (applyKnockback)
        {
            Vector2 dir = (other.transform.position - transform.position);
            if (dir.sqrMagnitude < 0.01f)
                dir = Vector2.up; // 최소 보정

            dir = dir.normalized;
            Vector2 knockback = dir * knockbackForce;

            // PlayerMovement 쪽 넉백 처리 우선
            var movement = other.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.ApplyKnockback(knockback);
                Debug.Log($"[DamageSource] {other.name}에 ApplyKnockback 호출: 방향 {dir}, 힘 {knockbackForce}");
            }
            else
            {
                // 없으면 Rigidbody2D 직접 처리
                Rigidbody2D rb = other.attachedRigidbody;
                if (rb != null)
                {
                    rb.AddForce(knockback, ForceMode2D.Impulse);
                    Debug.Log($"[DamageSource] {other.name}에 Rigidbody2D로 넉백 적용");
                }
                else
                {
                    Debug.LogWarning($"[DamageSource] {other.name}에 Rigidbody2D 없음 (넉백 불가)");
                }
            }
        }

        // 5) 투사체 등은 1회 히트 후 파괴
        if (destroyOnHit && !continuousDamage)
            Destroy(gameObject);
    }
}
