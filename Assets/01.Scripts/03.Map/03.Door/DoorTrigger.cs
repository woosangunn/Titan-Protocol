using UnityEngine;
using MyGame.Map;

/// <summary>
/// �� �浹 ���� �� ���� ���� Ŭ����
/// - �÷��̾ ���� ���� �ε����� �ƹ� �ϵ� �Ͼ�� ����
/// - ���� ���� �÷��̾ ������ RoomLoader�� ���� �� �̵� �õ�
/// </summary>
public class DoorTrigger : MonoBehaviour
{
    public Vector2Int direction;       // �� ���� ���ϰ� �ִ� ���� (��: ������ ���̸� (1, 0))
    public RoomLoader roomLoader;      // �� �̵��� ó���ϴ� RoomLoader ����

    private bool isOpen = false;       // ���� ���� �ִ� �������� ����

    /// <summary>
    /// �ܺο��� �� �޼��带 ȣ���ϸ� ���� ���� ���·� ������
    /// - ���� �浹 �� �� �̵� ����
    /// - �ִϸ��̼�, ���� ���� �� �ȿ��� �߰� ���� ����
    /// </summary>
    public void Open()
    {
        isOpen = true;
        // TODO: �ִϸ��̼� �Ǵ� ���� ����Ʈ �߰� ����
    }

    /// <summary>
    /// �浹 �̺�Ʈ ���� (�÷��̾ ���� ����� �� ����)
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DoorTrigger OnTriggerEnter2D by {other.name}");

        // ���� ���� ������ �ƹ� �͵� ���� ����
        if (!isOpen) return;

        // �浹�� ����� �÷��̾ �ƴϸ� ����
        if (!other.CompareTag("Player")) return;

        // �÷��̾ ���� ���� ������Ƿ� �ش� �������� �� �̵� �õ�
        roomLoader.TryMove(direction);
    }
}
