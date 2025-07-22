using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Transform[] aimPivots; // ������ ȸ�� ��� ������Ʈ�� (�ѱ�, ���� ��)
    Camera cam;

    void Start()
    {
        cam = Camera.main; // ���� ī�޶� ����
    }

    void Update()
    {
        // ���콺�� ���� ��ǥ ���ϱ�
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // ��� ���� ��� ���� �ݺ�
        foreach (Transform pivot in aimPivots)
        {
            if (pivot == null) continue; // �� üũ

            // pivot ��ġ���� ���콺 �������� ���� ���� ���
            Vector2 dir = (mouseWorldPos - (Vector2)pivot.position).normalized;

            // ���� ���͸� ������ ��ȯ (���� -> ��)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // pivot ȸ�� ���� (Z�� ���� 2D ȸ��)
            pivot.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
