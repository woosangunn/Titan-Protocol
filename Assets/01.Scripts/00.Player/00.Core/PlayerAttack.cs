using UnityEngine;

/// <summary>
/// 플레이어 공격 담당
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("발사 위치")]
    [Tooltip("총알 발사 위치")]
    public Transform gunMuzzle;
    [Tooltip("사슬 발사 위치")]
    public Transform chainMuzzle;

    [Header("프리팹 & 속도")]
    [Tooltip("플레이어 총알 프리팹")]
    public GameObject bulletPrefab;
    [Tooltip("사슬 프리팹")]
    public GameObject chainPrefab;
    [Tooltip("총알 발사 속도")]
    public float bulletSpeed = 10f;
    [Tooltip("사슬 발사 속도")]
    public float chainSpeed = 10f;
    [Tooltip("총알 재장전 시간")]
    public float reloadTime = 2f;


    private PlayerStatus status;               // 플레이어 상태 (탄약, 장전 등)
    private PlayerGrappleHandler grappleHandler; // 그랩(사슬) 처리 컴포넌트

    void Awake()
    {
        status = GetComponent<PlayerStatus>();
        grappleHandler = GetComponent<PlayerGrappleHandler>();

        if (status == null)
            Debug.LogError("[PlayerAttack] PlayerStatus를 찾지 못했습니다.");
        if (grappleHandler == null)
            Debug.LogError("[PlayerAttack] PlayerGrappleHandler를 찾지 못했습니다.");
    }

    void Update()
    {
        // R키 입력 시 장전 시작
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    /// <summary>
    /// 총 장전 함수 호출
    /// </summary>
    public void Reload()
    {
        if (status == null) return;
        status.StartReload(reloadTime); // PlayerStatus에서 장전 처리 시작
    }

    /// <summary>
    /// 총알 발사
    /// </summary>
    /// <param name="targetPos">발사 목표 위치 (월드 좌표)</param>
    public void ShootBullet(Vector2 targetPos)
    {
        // 상태 체크 : 상태 컴포넌트가 없거나, 탄약이 없거나, 장전 중이면 발사 불가
        if (status == null || status.CurrentAmmo <= 0 || status.IsReloading)
            return;

        Vector2 firePos = gunMuzzle.position;                 // 총구 위치
        Vector2 dir = (targetPos - firePos).normalized;       // 발사 방향 (정규화)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;  // 회전 각도 계산 (라디안 → 도)

        // 풀에서 총알 오브젝트를 꺼내서 위치 및 회전 설정
        Bullet bullet = BulletPool.Instance.GetBullet(BulletType.Player, firePos, Quaternion.Euler(0, 0, angle));

        // Rigidbody2D 컴포넌트를 찾아 linearVelocity를 설정해 총알 발사
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;

        status.UseAmmo(); // 탄약 소모 처리
    }

    /// <summary>
    /// 사슬 발사
    /// </summary>
    /// <param name="targetPos">발사 목표 위치 (월드 좌표)</param>
    public void ShootChain(Vector2 targetPos)
    {
        // 이미 사슬이 붙어있으면 사슬 발사 불가
        if (grappleHandler != null && grappleHandler.IsAttached)
            return;

        Vector2 firePos = chainMuzzle.position;                // 사슬 발사 위치
        Vector2 dir = (targetPos - firePos).normalized;        // 사슬 발사 방향

        // 사슬 프리팹 인스턴스 생성 및 방향 설정
        GameObject chain = Instantiate(chainPrefab, firePos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        chain.transform.rotation = Quaternion.Euler(0, 0, angle);

        // ChainProjectile 스크립트 초기화 (발사 방향과 콜백 전달)
        var chainScript = chain.GetComponent<ChainProjectile>();
        if (chainScript != null)
            chainScript.Init(dir, grappleHandler.AttachGrapple);
    }
}
