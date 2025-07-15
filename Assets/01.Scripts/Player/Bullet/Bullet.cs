using UnityEngine;

/// <summary>
/// 총알 동작을 제어하는 스크립트.
/// - 일정 시간이 지나거나 충돌하면 BulletPool로 되돌림.
/// - Rigidbody2D는 PlayerAttack에서 속도를 세팅해 줌.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;     // 자동 소멸까지 시간

    private void OnEnable()
    {
        // 매번 새로 활성화될 때 타이머 리셋
        CancelInvoke(nameof(Disable));
        Invoke(nameof(Disable), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: 적 피격 처리 등 추가 가능
        Disable();  // 충돌 즉시 풀에 반환
    }

    /// <summary>
    /// BulletPool로 반환
    /// </summary>
    private void Disable()
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}
