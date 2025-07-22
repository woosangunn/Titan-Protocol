using UnityEngine;

public class ChainProjectile : MonoBehaviour
{
    public float speed = 20f;               // 사슬 발사 속도
    public LayerMask grappleLayer;          // 사슬이 붙을 수 있는 레이어 마스크

    private Vector2 moveDir;                 // 발사 방향 단위 벡터
    private bool hasHit = false;             // 충돌 여부 플래그
    private System.Action<Vector2> onHitCallback;  // 충돌 시 호출할 콜백 함수

    /// <summary>
    /// 사슬 발사 초기화: 방향과 충돌 시 실행할 콜백 함수 전달
    /// </summary>
    public void Init(Vector2 direction, System.Action<Vector2> onHit)
    {
        moveDir = direction.normalized;      // 방향 정규화
        onHitCallback = onHit;                // 콜백 저장
    }

    void Update()
    {
        if (!hasHit)
            // 충돌 전까지 일정 속도로 이동
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;  // 이미 충돌했으면 무시

        // 충돌한 오브젝트의 레이어가 grappleLayer에 포함되는지 체크
        if (((1 << other.gameObject.layer) & grappleLayer) != 0)
        {
            hasHit = true;                          // 충돌 처리 플래그 설정
            onHitCallback?.Invoke(transform.position);  // 충돌 위치 콜백 호출
            Destroy(gameObject);                    // 사슬 발사체 제거
        }
    }
}
