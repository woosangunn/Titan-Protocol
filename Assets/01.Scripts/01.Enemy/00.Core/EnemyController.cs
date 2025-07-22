using UnityEngine;

/// <summary>
/// �� ���� ��Ʈ�ѷ� ������Ʈ
/// - EnemyStats ���������� ���� ����
/// - EnemyBrain�� ���� �ൿ ����
/// - IDamageable �������� ���� ó�� �� ��� ����
/// </summary>
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [Tooltip("�⺻ ���� ���ø� (ScriptableObject)")]
    public EnemyStats baseStats; // ���ø�

    [HideInInspector]
    [Tooltip("�ν��Ͻ��� ���� ������")]
    public EnemyStats Stats; // ������

    [Tooltip("�� AI �γ� ������Ʈ")]
    public EnemyBrain brain;

    /// <summary>
    /// ������ ���� �ð� (��Ÿ�� ������)
    /// </summary>
    public float LastAttackTime { get; set; } // ���� ��Ÿ��

    private void Awake()
    {
        if (baseStats == null)
        {
            Debug.LogError("[EnemyController] baseStats�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // �⺻ ������ �����Ͽ� �ν��Ͻ��� ���·� ���
        Stats = baseStats.Clone();
    }

    /// <summary>
    /// IDamageable ���� - ���� �Ա�
    /// </summary>
    /// <param name="amount">���� ���ط�</param>
    public void TakeDamage(int amount)
    {
        Stats.CurrentHealth -= amount;

        if (Stats.CurrentHealth <= 0)
            Die();
    }

    /// <summary>
    /// �� ��� ó��
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        // �� ������ EnemyBrain�� Tick ȣ���Ͽ� �ൿ ó��
        brain?.Tick(this);
    }
}
