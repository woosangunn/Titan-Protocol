using UnityEngine;

/// <summary>
/// �÷��̾� ���� ���
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("�߻� ��ġ")]
    [Tooltip("�Ѿ� �߻� ��ġ")]
    public Transform gunMuzzle;
    [Tooltip("�罽 �߻� ��ġ")]
    public Transform chainMuzzle;

    [Header("������ & �ӵ�")]
    [Tooltip("�÷��̾� �Ѿ� ������")]
    public GameObject bulletPrefab;
    [Tooltip("�罽 ������")]
    public GameObject chainPrefab;
    [Tooltip("�Ѿ� �߻� �ӵ�")]
    public float bulletSpeed = 10f;
    [Tooltip("�罽 �߻� �ӵ�")]
    public float chainSpeed = 10f;
    [Tooltip("�Ѿ� ������ �ð�")]
    public float reloadTime = 2f;


    private PlayerStatus status;               // �÷��̾� ���� (ź��, ���� ��)
    private PlayerGrappleHandler grappleHandler; // �׷�(�罽) ó�� ������Ʈ

    void Awake()
    {
        status = GetComponent<PlayerStatus>();
        grappleHandler = GetComponent<PlayerGrappleHandler>();

        if (status == null)
            Debug.LogError("[PlayerAttack] PlayerStatus�� ã�� ���߽��ϴ�.");
        if (grappleHandler == null)
            Debug.LogError("[PlayerAttack] PlayerGrappleHandler�� ã�� ���߽��ϴ�.");
    }

    void Update()
    {
        // RŰ �Է� �� ���� ����
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    /// <summary>
    /// �� ���� �Լ� ȣ��
    /// </summary>
    public void Reload()
    {
        if (status == null) return;
        status.StartReload(reloadTime); // PlayerStatus���� ���� ó�� ����
    }

    /// <summary>
    /// �Ѿ� �߻�
    /// </summary>
    /// <param name="targetPos">�߻� ��ǥ ��ġ (���� ��ǥ)</param>
    public void ShootBullet(Vector2 targetPos)
    {
        // ���� üũ : ���� ������Ʈ�� ���ų�, ź���� ���ų�, ���� ���̸� �߻� �Ұ�
        if (status == null || status.CurrentAmmo <= 0 || status.IsReloading)
            return;

        Vector2 firePos = gunMuzzle.position;                 // �ѱ� ��ġ
        Vector2 dir = (targetPos - firePos).normalized;       // �߻� ���� (����ȭ)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;  // ȸ�� ���� ��� (���� �� ��)

        // Ǯ���� �Ѿ� ������Ʈ�� ������ ��ġ �� ȸ�� ����
        Bullet bullet = BulletPool.Instance.GetBullet(BulletType.Player, firePos, Quaternion.Euler(0, 0, angle));

        // Rigidbody2D ������Ʈ�� ã�� linearVelocity�� ������ �Ѿ� �߻�
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;

        status.UseAmmo(); // ź�� �Ҹ� ó��
    }

    /// <summary>
    /// �罽 �߻�
    /// </summary>
    /// <param name="targetPos">�߻� ��ǥ ��ġ (���� ��ǥ)</param>
    public void ShootChain(Vector2 targetPos)
    {
        // �̹� �罽�� �پ������� �罽 �߻� �Ұ�
        if (grappleHandler != null && grappleHandler.IsAttached)
            return;

        Vector2 firePos = chainMuzzle.position;                // �罽 �߻� ��ġ
        Vector2 dir = (targetPos - firePos).normalized;        // �罽 �߻� ����

        // �罽 ������ �ν��Ͻ� ���� �� ���� ����
        GameObject chain = Instantiate(chainPrefab, firePos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        chain.transform.rotation = Quaternion.Euler(0, 0, angle);

        // ChainProjectile ��ũ��Ʈ �ʱ�ȭ (�߻� ����� �ݹ� ����)
        var chainScript = chain.GetComponent<ChainProjectile>();
        if (chainScript != null)
            chainScript.Init(dir, grappleHandler.AttachGrapple);
    }
}
