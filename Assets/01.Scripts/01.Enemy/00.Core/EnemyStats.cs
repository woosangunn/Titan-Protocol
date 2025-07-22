using UnityEngine;

/// <summary>
/// ���� ���� ���� ScriptableObject
/// - �ִ� ü��, �̵� �ӵ�, ���ݷ�, ���� ��Ÿ��, ���� �Ÿ� ����
/// - ��Ÿ�ӿ� ���� ü�� ����
/// - ����(Ŭ��) ��� ����
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    [Tooltip("�ִ� ü��")]
    public int MaxHealth = 100;
    [Tooltip("�̵� �ӵ�")]
    public float MoveSpeed = 2f;
    [Tooltip("���ݷ�")]
    public int Damage = 10;
    [Tooltip("���� ��Ÿ�� (��)")]
    public float AttackCooldown = 1f;
    [Tooltip("���� ��Ÿ�")]
    public float AttackRange = 1.5f;

    [HideInInspector]
    [Tooltip("���� ü�� (��Ÿ�ӿ��� ���)")]
    public int CurrentHealth;

    /// <summary>
    /// ��Ÿ�� ���� �� �ʱ�ȭ�� �޼���
    /// - ���� ü���� �ִ� ü������ ����
    /// </summary>
    public void InitializeRuntimeValues()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// �� ScriptableObject�� �����Ͽ� ���ο� �ν��Ͻ� ����
    /// - �������� ��Ÿ�ӿ��� ���������� ��� ����
    /// </summary>
    /// <returns>������ EnemyStats ��ü</returns>
    public EnemyStats Clone()
    {
        EnemyStats clone = Instantiate(this);
        clone.InitializeRuntimeValues();
        return clone;
    }
}
