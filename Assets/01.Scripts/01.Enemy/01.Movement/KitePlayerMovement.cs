using UnityEngine;

/// <summary>
/// �÷��̾ ���� �Ÿ� �����ϸ� �����ϰų� �����ϴ� ���� �̵� ����
/// - �ּ� �Ÿ� �̳��� ����, �ִ� �Ÿ� �̻��̸� ����
/// - ���� ��Ÿ� ���� ������ ����
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Movement/Kite Player")]
public class KitePlayerMovement : EnemyMovementBase
{
    /// <summary>
    /// �÷��̾�� �ּ� ���� �Ÿ� (�� �Ÿ����� ������ ����)
    /// </summary>
    public float minDistance = 4f;

    /// <summary>
    /// �÷��̾�� �ִ� ���� �Ÿ� (�� �Ÿ����� �ָ� ����)
    /// </summary>
    public float maxDistance = 6f;

    /// <summary>
    /// �� ������ ȣ��Ǿ� �÷��̾���� �Ÿ��� üũ�ϰ� ���� �Ǵ� ���� �̵� ����
    /// </summary>
    /// <param name="controller">�̵��� ������ EnemyController �ν��Ͻ�</param>
    public override void Move(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 dirToPlayer = player.transform.position - controller.transform.position;
        float distance = dirToPlayer.magnitude;
        Vector2 direction = dirToPlayer.normalized;

        if (distance < minDistance)
        {
            // �÷��̾�� �ʹ� ������ ����
            controller.transform.position -= (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
        else if (distance > maxDistance)
        {
            // �÷��̾�� �ʹ� �ָ� ����
            controller.transform.position += (Vector3)(direction * controller.Stats.MoveSpeed * Time.deltaTime);
        }
        // minDistance �̻� maxDistance ������ ���� ���� (�̵� ����)
    }
}
