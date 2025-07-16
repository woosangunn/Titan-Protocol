using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("�̵�/���")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    [Header("�˹�")]
    public float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 dashDirection;

    private bool isDashing = false;
    private float dashTimer = 0f;

    private bool isKnockbacked = false;
    private float knockbackTimer = 0f;

    private PlayerGrappleHandler grapple;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        grapple = GetComponent<PlayerGrappleHandler>();
    }

    void FixedUpdate()
    {
        // �罽�� �پ������� �������� ����
        if (grapple != null && grapple.IsAttached) return;

        // �˹� ���̸� �ƹ� �Էµ� ���� ����
        if (isKnockbacked)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
                isKnockbacked = false;

            return;
        }

        // ��� ���� ��
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
                isDashing = false;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    /// <summary>
    /// �̵� ���� �Է� (�˹�/��� �߿��� ���õ�)
    /// </summary>
    public void SetMoveDirection(Vector2 dir)
    {
        if (isDashing || isKnockbacked) return;
        moveInput = dir;
    }

    /// <summary>
    /// ��� ����
    /// </summary>
    public void StartDash(Vector2 dir)
    {
        if (dir == Vector2.zero || isKnockbacked) return;

        isDashing = true;
        dashDirection = dir.normalized;
        dashTimer = dashDuration;
    }

    /// <summary>
    /// ��� ����
    /// </summary>
    public void StopImmediately()
    {
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        isKnockbacked = false;
    }

    /// <summary>
    /// �ܺο��� �˹� ���� (�˹� �� �̵�/��� ����)
    /// </summary>
    public void ApplyKnockback(Vector2 force)
    {
        isKnockbacked = true;
        knockbackTimer = knockbackDuration;

        rb.linearVelocity = Vector2.zero; // ���� �ӵ� ����
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
