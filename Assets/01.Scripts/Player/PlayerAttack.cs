using UnityEngine;

/// <summary>
/// 플레이어 공격 담당
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("발사 위치")]
    public Transform gunMuzzle;
    public Transform chainMuzzle;

    [Header("프리팹 & 속도")]
    public GameObject bulletPrefab;
    public GameObject chainPrefab;
    public float bulletSpeed = 10f;
    public float chainSpeed = 10f;

    public float reloadTime = 2f; // 장전 시간 설정

    private PlayerStatus status;
    private PlayerGrappleHandler grappleHandler;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (status == null) return;
        status.StartReload(reloadTime); // 장전 실행
    }

    public void ShootBullet(Vector2 targetPos)
    {
        if (status == null || status.CurrentAmmo <= 0 || status.IsReloading)
            return;

        Vector2 firePos = gunMuzzle.position;
        Vector2 dir = (targetPos - firePos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject bullet = BulletPool.Instance.GetBullet(firePos, Quaternion.Euler(0, 0, angle));

        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;

        status.UseAmmo();
    }

    public void ShootChain(Vector2 targetPos)
    {
        if (grappleHandler != null && grappleHandler.IsAttached)
            return;

        Vector2 firePos = chainMuzzle.position;
        Vector2 dir = (targetPos - firePos).normalized;

        GameObject chain = Instantiate(chainPrefab, firePos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        chain.transform.rotation = Quaternion.Euler(0, 0, angle);

        var chainScript = chain.GetComponent<ChainProjectile>();
        if (chainScript != null)
            chainScript.Init(dir, grappleHandler.AttachGrapple);
    }
}
