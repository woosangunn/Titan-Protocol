using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;        // 총알이 활성화된 후 자동 비활성화까지의 시간
    public BulletType bulletType;      // 총알 타입 구분 (플레이어용, 적용 등)

    private void OnEnable()
    {
        CancelInvoke(nameof(Disable)); // 기존 예약된 Disable 호출 취소
        Invoke(nameof(Disable), lifeTime); // lifeTime 초 후 자동으로 비활성화 예약
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // DamageSource 컴포넌트가 없으면 (예: 벽과 충돌 등)
        // 즉시 비활성화 처리 (총알 제거)
        if (GetComponent<DamageSource>() == null)
            Disable();
    }

    private void Disable()
    {
        // 총알을 풀에 반환 (재활용을 위해 비활성화 처리)
        BulletPool.Instance.ReturnBullet(this);
    }
}
