using UnityEngine;

public class ChainProjectile : MonoBehaviour
{
    public float speed = 20f;               // �罽 �߻� �ӵ�
    public LayerMask grappleLayer;          // �罽�� ���� �� �ִ� ���̾� ����ũ

    private Vector2 moveDir;                 // �߻� ���� ���� ����
    private bool hasHit = false;             // �浹 ���� �÷���
    private System.Action<Vector2> onHitCallback;  // �浹 �� ȣ���� �ݹ� �Լ�

    // �߻� �ʱ�ȭ: ����� �浹 �� ������ �ݹ� ���޹���
    public void Init(Vector2 direction, System.Action<Vector2> onHit)
    {
        moveDir = direction.normalized;      // ���� ���� ���ͷ� ����ȭ
        onHitCallback = onHit;                // �ݹ� �Լ� ����
    }

    void Update()
    {
        if (!hasHit)
            // �浹 ������ �������� ���� �ӵ� �̵�
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;  // �̹� �浹������ ����

        // �浹�� ������Ʈ�� ���̾ grappleLayer�� ���ԵǾ� �ִ��� �˻�
        if (((1 << other.gameObject.layer) & grappleLayer) != 0)
        {
            hasHit = true;                          // �浹 ���·� ����
            onHitCallback?.Invoke(transform.position);  // �浹 ���� ��ǥ�� �ݹ鿡 ����
            Destroy(gameObject);                    // �罽 �߻�ü ����
        }
    }
}
