using UnityEngine;

/// <summary>
/// �÷��̾ �Ѿư��� ���� �̵� ����
/// - �÷��̾� �������� �̵�
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Movement/Chase Player")]
public class ChasePlayerMovement : EnemyMovementBase
{
    /// <summary>
    /// �� ������ ȣ��Ǿ� �÷��̾� �������� �̵� ó��
    /// </summary>
    /// <param name="controller">�̵��� ������ EnemyController �ν��Ͻ�</param>
    public override void Move(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 direction = (player.transform.position - controller.transform.position).normalized;
        controller.transform.position += (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
    }
}
