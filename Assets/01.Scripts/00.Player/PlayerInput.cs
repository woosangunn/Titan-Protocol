using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // �÷��̾��� ���� �̵� ���� (����ȭ�� ����)
    public Vector2 Move { get; private set; }

    // ���������� �Էµ� �̵� ���� (�̵� �Է��� ���� ���� ����)
    public Vector2 LastMove { get; private set; }

    // ��� �Է� ���� (�����̽��ٰ� ������ �� true)
    public bool DashPressed { get; private set; }

    // ���� ���콺 Ŭ�� ���� (Ŭ���� �߻��� �����ӿ� true)
    public bool LeftClick { get; private set; }

    // ������ ���콺 Ŭ�� ���� (Ŭ���� �߻��� �����ӿ� true)
    public bool RightClick { get; private set; }

    // ���� ���콺 Ŀ���� ���� ��ǥ (2D ��� ����)
    public Vector2 MouseWorldPos2D => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    // �� ������ ȣ��Ǿ� �÷��̾� �Է� ���¸� ����
    public void HandleInput()
    {
        float h = 0f, v = 0f;

        // �¿� �̵� �Է� üũ (A, D Ű)
        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;

        // ���� �̵� �Է� üũ (W, S Ű)
        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;

        // �Է� ���� ���� �� ����ȭ (�밢�� �̵� �� �ӵ� ����ȭ)
        Vector2 input = new Vector2(h, v);
        Move = input.normalized;

        // �̵� �Է��� ���� ��� ������ �̵� ���� ����
        if (input != Vector2.zero)
            LastMove = Move;

        // �����̽��� ���� ���� üũ (�� �����Ӹ� true)
        DashPressed = Input.GetKeyDown(KeyCode.Space);

        // ���콺 ���� ��ư Ŭ�� üũ (�� �����Ӹ� true)
        LeftClick = Input.GetMouseButtonDown(0);

        // ���콺 ������ ��ư Ŭ�� üũ (�� �����Ӹ� true)
        RightClick = Input.GetMouseButtonDown(1);
    }
}
