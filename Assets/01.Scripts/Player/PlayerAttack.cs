using UnityEngine;

/// <summary>
/// �÷��̾� ���� ���
/// - �Ѿ� �߻�: ź�� 1�� �Һ� �� PlayerStatus �̺�Ʈ�� UI �ڵ� ����
/// - �罽 �߻�: GrappleHandler�� ����� ���� ���� ���� ����
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("�߻� ��ġ")]
    public Transform gunMuzzle;     // �Ѿ� �߻� ��ġ
    public Transform chainMuzzle;   // �罽 �߻� ��ġ

    [Header("������ & �ӵ�")]
    public GameObject bulletPrefab; // �Ѿ� ������
    public GameObject chainPrefab;  // �罽 �߻�ü ������
    public float bulletSpeed = 10f; // �Ѿ� �ӵ�
    public float chainSpeed = 10f; // (�̻��) �罽 �߻� �ӵ�

    private PlayerStatus status;          // ź�ࡤü�� ����
    private PlayerGrappleHandler grappleHandler; // �罽 ����

    void Awake()
    {
        // �÷��̾� ������Ʈ�� ���� ������Ʈ ����
        status = GetComponent<PlayerStatus>();
        grappleHandler = GetComponent<PlayerGrappleHandler>();

        if (status == null)
            Debug.LogError("[PlayerAttack] PlayerStatus�� ã�� ���߽��ϴ�.");
        if (grappleHandler == null)
            Debug.LogError("[PlayerAttack] PlayerGrappleHandler�� ã�� ���߽��ϴ�.");
    }

    /// <summary>
    /// ���� Ŭ��: �Ѿ� �߻�
    /// </summary>

    public void ShootBullet(Vector2 targetPos)
    {
        if (status == null || status.CurrentAmmo <= 0)
            return; // ź�� ���� �� �߻� ���

        // ���� ���
        Vector2 firePos = gunMuzzle.position;
        Vector2 dir = (targetPos - firePos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // ������Ʈ Ǯ���� �Ѿ� ������
        GameObject bullet = BulletPool.Instance.GetBullet(firePos, Quaternion.Euler(0, 0, angle));

        // �ӵ� �ο�
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;

        // ź�� 1�� �Һ� �� UI �ڵ� ����
        status.UseAmmo();
    }


    /// <summary>
    /// ������ Ŭ��: �罽 �߻�
    /// </summary>
    public void ShootChain(Vector2 targetPos)
    {
        // �̹� �罽�� �پ� ������ �߻� �Ұ�
        if (grappleHandler != null && grappleHandler.IsAttached)
            return;

        Vector2 firePos = chainMuzzle.position;
        Vector2 dir = (targetPos - firePos).normalized;

        GameObject chain = Instantiate(chainPrefab, firePos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        chain.transform.rotation = Quaternion.Euler(0, 0, angle);

        // ChainProjectile�� �߻�ü �̵����浹 �� AttachGrapple ȣ��
        var chainScript = chain.GetComponent<ChainProjectile>();
        if (chainScript != null)
            chainScript.Init(dir, grappleHandler.AttachGrapple);
    }
}
