using UnityEngine;

/// <summary>
/// �Ѿ� ������ �����ϴ� ��ũ��Ʈ.
/// - ���� �ð��� �����ų� �浹�ϸ� BulletPool�� �ǵ���.
/// - Rigidbody2D�� PlayerAttack���� �ӵ��� ������ ��.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;     // �ڵ� �Ҹ���� �ð�

    private void OnEnable()
    {
        // �Ź� ���� Ȱ��ȭ�� �� Ÿ�̸� ����
        CancelInvoke(nameof(Disable));
        Invoke(nameof(Disable), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: �� �ǰ� ó�� �� �߰� ����
        Disable();  // �浹 ��� Ǯ�� ��ȯ
    }

    /// <summary>
    /// BulletPool�� ��ȯ
    /// </summary>
    private void Disable()
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}
