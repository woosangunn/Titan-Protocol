using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // ���� ��� (�÷��̾� ��)
    public float smoothSpeed = 5f;     // ī�޶� �̵� �ε巯�� �ӵ�
    public Vector2 offset;             // ���� ī�޶� �� ��ġ ������

    void LateUpdate()
    {
        if (target == null) return;   // ����� ������ �������� ����

        // ��� ��ġ�� �������� ���� ��ǥ ��ġ ���
        Vector3 desiredPosition = target.position + (Vector3)offset;

        // ���� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵� (z���� ī�޶� ���� -10)
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(desiredPosition.x, desiredPosition.y, -10f),
            smoothSpeed * Time.deltaTime);
    }
}
