using UnityEngine;

/// <summary>
/// �������� �ʴ� ������ �� �̵� ����
/// - ���� ���� ��ġ�� �ӹ������� ��
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Movement/Static")]
public class StaticMovement : EnemyMovementBase
{
    /// <summary>
    /// �̵� ���� (����)
    /// </summary>
    /// <param name="controller">�̵��� ������ EnemyController �ν��Ͻ�</param>
    public override void Move(EnemyController controller)
    {
        // �̵����� ����
    }
}
