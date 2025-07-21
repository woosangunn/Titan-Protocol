using UnityEngine;

public class CameraMouseOffset : MonoBehaviour
{
    public Transform target;        // ���� ��� (�÷��̾� ��)
    public float followSpeed = 5f;  // ī�޶� �̵� �ε巯�� �ӵ�
    public float maxOffset = 5f;    // ���콺 ��ġ�� ���� �ִ� ������ �Ÿ�

    Camera cam;

    void Start()
    {
        cam = Camera.main;          // ���� ī�޶� ����
    }

    void LateUpdate()
    {
        if (!target) return;        // ����� ������ �������� ����

        // ���콺 ȭ�� ��ǥ �� ���� ��ǥ ��ȯ
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = target.position;

        // ��� ��ġ�� ���콺 ��ġ �� ���� �� �Ÿ� ���
        Vector2 offsetDir = mouseWorldPos - targetPos;

        // �������� maxOffset���� ũ�� �ִ밪���� ����
        if (offsetDir.magnitude > maxOffset)
            offsetDir = offsetDir.normalized * maxOffset;

        // ��� ��ġ�� ���ѵ� ������ ���ϱ�
        Vector3 desiredPos = targetPos + (Vector3)offsetDir;

        // ī�޶� ��ġ�� �ε巴�� ��ǥ ��ġ�� �̵� (z�� ���� -10)
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(desiredPos.x, desiredPos.y, -10f),
            followSpeed * Time.deltaTime);
    }
}
