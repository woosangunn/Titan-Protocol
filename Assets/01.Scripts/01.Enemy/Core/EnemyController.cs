using UnityEngine;

/// <summary>
/// 적 개별 컨트롤러 컴포넌트
/// - EnemyStats 복제본으로 상태 관리
/// - EnemyBrain을 통해 행동 수행
/// - IDamageable 구현으로 피해 처리 및 사망 관리
/// </summary>
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [Tooltip("기본 스탯 템플릿 (ScriptableObject)")]
    public EnemyStats baseStats; // 템플릿

    [HideInInspector]
    [Tooltip("인스턴스용 스탯 복제본")]
    public EnemyStats Stats; // 복제본

    [Tooltip("적 AI 두뇌 컴포넌트")]
    public EnemyBrain brain;

    /// <summary>
    /// 마지막 공격 시각 (쿨타임 관리용)
    /// </summary>
    public float LastAttackTime { get; set; } // 개별 쿨타임

    private void Awake()
    {
        if (baseStats == null)
        {
            Debug.LogError("[EnemyController] baseStats가 설정되지 않았습니다.");
            return;
        }

        // 기본 스탯을 복제하여 인스턴스별 상태로 사용
        Stats = baseStats.Clone();
    }

    /// <summary>
    /// IDamageable 구현 - 피해 입기
    /// </summary>
    /// <param name="amount">입을 피해량</param>
    public void TakeDamage(int amount)
    {
        Stats.CurrentHealth -= amount;

        if (Stats.CurrentHealth <= 0)
            Die();
    }

    /// <summary>
    /// 적 사망 처리
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        // 매 프레임 EnemyBrain의 Tick 호출하여 행동 처리
        brain?.Tick(this);
    }
}
