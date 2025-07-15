using UnityEngine;

/// <summary>
/// 플레이어 공격 담당
/// - 총알 발사: 탄약 1발 소비 → PlayerStatus 이벤트로 UI 자동 갱신
/// - 사슬 발사: GrappleHandler가 연결돼 있지 않을 때만 실행
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("발사 위치")]
    public Transform gunMuzzle;     // 총알 발사 위치
    public Transform chainMuzzle;   // 사슬 발사 위치

    [Header("프리팹 & 속도")]
    public GameObject bulletPrefab; // 총알 프리팹
    public GameObject chainPrefab;  // 사슬 발사체 프리팹
    public float bulletSpeed = 10f; // 총알 속도
    public float chainSpeed = 10f; // (미사용) 사슬 발사 속도

    private PlayerStatus status;          // 탄약·체력 상태
    private PlayerGrappleHandler grappleHandler; // 사슬 조작

    void Awake()
    {
        // 플레이어 오브젝트에 붙은 컴포넌트 참조
        status = GetComponent<PlayerStatus>();
        grappleHandler = GetComponent<PlayerGrappleHandler>();

        if (status == null)
            Debug.LogError("[PlayerAttack] PlayerStatus를 찾지 못했습니다.");
        if (grappleHandler == null)
            Debug.LogError("[PlayerAttack] PlayerGrappleHandler를 찾지 못했습니다.");
    }

    /// <summary>
    /// 왼쪽 클릭: 총알 발사
    /// </summary>

    public void ShootBullet(Vector2 targetPos)
    {
        if (status == null || status.CurrentAmmo <= 0)
            return; // 탄약 부족 시 발사 취소

        // 방향 계산
        Vector2 firePos = gunMuzzle.position;
        Vector2 dir = (targetPos - firePos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 오브젝트 풀에서 총알 꺼내기
        GameObject bullet = BulletPool.Instance.GetBullet(firePos, Quaternion.Euler(0, 0, angle));

        // 속도 부여
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;

        // 탄약 1발 소비 → UI 자동 갱신
        status.UseAmmo();
    }


    /// <summary>
    /// 오른쪽 클릭: 사슬 발사
    /// </summary>
    public void ShootChain(Vector2 targetPos)
    {
        // 이미 사슬이 붙어 있으면 발사 불가
        if (grappleHandler != null && grappleHandler.IsAttached)
            return;

        Vector2 firePos = chainMuzzle.position;
        Vector2 dir = (targetPos - firePos).normalized;

        GameObject chain = Instantiate(chainPrefab, firePos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        chain.transform.rotation = Quaternion.Euler(0, 0, angle);

        // ChainProjectile이 발사체 이동·충돌 후 AttachGrapple 호출
        var chainScript = chain.GetComponent<ChainProjectile>();
        if (chainScript != null)
            chainScript.Init(dir, grappleHandler.AttachGrapple);
    }
}
