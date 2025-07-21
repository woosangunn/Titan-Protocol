using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("�̵�/���")]
    [Tooltip("�÷��̾� �̵� �ӵ�")]
    public float moveSpeed = 5f;
    [Tooltip("��� �ӵ�")]
    public float dashSpeed = 15f;
    [Tooltip("��� ���� �ð�")]
    public float dashDuration = 0.2f;

    [Header("�˹�")]
    [Tooltip("�˹� ���� �ð�")]
    public float knockbackDuration = 0.2f;


    private Rigidbody2D rb;                // ���� �̵��� ���� Rigidbody2D ������Ʈ
    private Vector2 moveInput;             // �÷��̾ �Է��� �̵� ����
    private Vector2 dashDirection;         // ��� ����

    private bool isDashing = false;        // ��� �� ���� ����
    private float dashTimer = 0f;          // ��� �ð� ī����

    private bool isKnockbacked = false;    // �˹� �� ���� ����
    private float knockbackTimer = 0f;     // �˹� �ð� ī����

    private PlayerGrappleHandler grapple;  // �罽 �׷� �ڵ鷯 (�پ������� �̵� ���� ����)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;                           // �߷� ����
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // ȸ�� ����

        grapple = GetComponent<PlayerGrappleHandler>();
    }

    void FixedUpdate()
    {
        // �罽�� �پ������� �̵�/��� ��� ���� ����
        if (grapple != null && grapple.IsAttached) return;

        // �˹� ������ ��
        if (isKnockbacked)
        {
            knockbackTimer -= Time.fixedDeltaTime;   // �˹� �ð� ����
            if (knockbackTimer <= 0f)
                isKnockbacked = false;                // �˹� ����

            return; // �˹� �߿��� �̵��̳� ��� ����
        }

        // ��� ���� ��
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;  // ��� ����� �ӵ��� �̵�
            dashTimer -= Time.fixedDeltaTime;         // ��� �ð� ����
            if (dashTimer <= 0f)
                isDashing = false;                     // ��� ����
        }
        else
        {
            // �Ϲ� �̵� ó��
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    /// <summary>
    /// �̵� ���� ���� (��ó� �˹� �߿��� �Է� ����)
    /// </summary>
    public void SetMoveDirection(Vector2 dir)
    {
        if (isDashing || isKnockbacked) return;
        moveInput = dir;
    }

    /// <summary>
    /// ��� ���� �Լ�
    /// </summary>
    public void StartDash(Vector2 dir)
    {
        if (dir == Vector2.zero || isKnockbacked) return; // �˹� ���̰ų� ������ ������ ��� �� ��

        isDashing = true;                 // ��� ���� Ȱ��ȭ
        dashDirection = dir.normalized;  // ��� ���� ����ȭ
        dashTimer = dashDuration;        // ��� �ð� �ʱ�ȭ
    }

    /// <summary>
    /// ��� �̵� ���� (���, �˹� ���µ� �ʱ�ȭ)
    /// </summary>
    public void StopImmediately()
    {
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        isKnockbacked = false;
    }

    /// <summary>
    /// �˹� ȿ�� ���� (�˹� ���� ����, ���� ����)
    /// </summary>
    public void ApplyKnockback(Vector2 force)
    {
        isKnockbacked = true;
        knockbackTimer = knockbackDuration;

        rb.linearVelocity = Vector2.zero;          // ���� �ӵ� �ʱ�ȭ
        rb.AddForce(force, ForceMode2D.Impulse); // �˹� �� ��� ���ϱ�
    }
}
