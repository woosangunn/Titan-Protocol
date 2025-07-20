using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public BulletType bulletType; // �÷��̾�/�� ���п�

    private void OnEnable()
    {
        CancelInvoke(nameof(Disable));
        Invoke(nameof(Disable), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GetComponent<DamageSource>() == null)
            Disable(); // ��: �� �浹 ��
    }

    private void Disable()
    {
        BulletPool.Instance.ReturnBullet(this);
    }
}
