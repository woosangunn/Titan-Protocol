using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("References")]
    public EnemyStats baseStats;      // 템플릿
    [HideInInspector]
    public EnemyStats Stats;          // 복제본
    public EnemyBrain brain;

    public float LastAttackTime { get; set; }  // 쿨타임 관리용

    private void Awake()
    {
        if (baseStats == null)
        {
            Debug.LogError("[EnemyController] baseStats가 설정되지 않았습니다.");
            return;
        }

        Stats = baseStats.Clone();
    }

    public void TakeDamage(int amount)
    {
        Stats.CurrentHealth -= amount;

        if (Stats.CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        brain?.Tick(this);
    }
}
