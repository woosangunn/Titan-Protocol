using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;        // �Ѿ��� Ȱ��ȭ�� �� �ڵ� ��Ȱ��ȭ������ �ð�
    public BulletType bulletType;      // �Ѿ� Ÿ�� ���� (�÷��̾��, ���� ��)

    private void OnEnable()
    {
        CancelInvoke(nameof(Disable)); // ���� ����� Disable ȣ�� ���
        Invoke(nameof(Disable), lifeTime); // lifeTime �� �� �ڵ����� ��Ȱ��ȭ ����
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // DamageSource ������Ʈ�� ������ (��: ���� �浹 ��)
        // ��� ��Ȱ��ȭ ó�� (�Ѿ� ����)
        if (GetComponent<DamageSource>() == null)
            Disable();
    }

    private void Disable()
    {
        // �Ѿ��� Ǯ�� ��ȯ (��Ȱ���� ���� ��Ȱ��ȭ ó��)
        BulletPool.Instance.ReturnBullet(this);
    }
}
