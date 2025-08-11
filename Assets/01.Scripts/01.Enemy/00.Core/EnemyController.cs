using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("References")]
    public EnemyStats baseStats;      // ���ø�
    [HideInInspector]
    public EnemyStats Stats;          // ������
    public EnemyBrain brain;

    public float LastAttackTime { get; set; }  // ��Ÿ�� ������

    private void Awake()
    {
        if (baseStats == null)
        {
            Debug.LogError("[EnemyController] baseStats�� �������� �ʾҽ��ϴ�.");
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
