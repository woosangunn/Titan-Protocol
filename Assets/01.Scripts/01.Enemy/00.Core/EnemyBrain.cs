using UnityEngine;

/// <summary>
/// �� AI�� �γ� ������ �ϴ� ������Ʈ
/// - EnemyMovementBase�� EnemyAttackBase�� �����Ͽ� �ൿ ���� �� ����
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    [Tooltip("�� �̵� ���� ������Ʈ")]
    public EnemyMovementBase movement;

    [Tooltip("�� ���� ���� ������Ʈ")]
    public EnemyAttackBase attack;

    /// <summary>
    /// �� ��Ʈ�ѷ��� �޾� �̵��� ������ ����
    /// </summary>
    /// <param name="controller">�� ���� �� ������ ��� ��Ʈ�ѷ�</param>
    public void Tick(EnemyController controller)
    {
        movement?.Move(controller);
        attack?.Attack(controller);
    }
}
