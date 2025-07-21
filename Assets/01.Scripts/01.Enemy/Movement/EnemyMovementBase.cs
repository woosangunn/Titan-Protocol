using UnityEngine;

/// <summary>
/// ���� �̵� ������ �����ϴ� �߻� �⺻ Ŭ����
/// - EnemyController�� �̵� ó���� �����ϴ� ������Ʈ
/// - ScriptableObject�� �����Ͽ� �پ��� �̵� ������ ���� ���� ����
/// </summary>
public abstract class EnemyMovementBase : ScriptableObject
{
    /// <summary>
    /// �� ������ EnemyController�� �̵��� ó���ϴ� �߻� �޼���
    /// </summary>
    /// <param name="controller">�̵��� ������ EnemyController �ν��Ͻ�</param>
    public abstract void Move(EnemyController controller);
}
