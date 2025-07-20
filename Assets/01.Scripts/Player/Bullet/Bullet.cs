using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public BulletType bulletType; // 플레이어/적 구분용

    private void OnEnable()
    {
        CancelInvoke(nameof(Disable));
        Invoke(nameof(Disable), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GetComponent<DamageSource>() == null)
            Disable(); // 예: 벽 충돌 등
    }

    private void Disable()
    {
        BulletPool.Instance.ReturnBullet(this);
    }
}
