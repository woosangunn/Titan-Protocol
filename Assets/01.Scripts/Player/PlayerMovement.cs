using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동/대시")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    [Header("넉백")]
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
        // 사슬에 붙어있으면 제어하지 않음
        if (grapple != null && grapple.IsAttached) return;

        // 넉백 중이면 아무 입력도 받지 않음
        if (isKnockbacked)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
                isKnockbacked = false;

            return;
        }

        // 대시 중일 때
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
    /// 이동 방향 입력 (넉백/대시 중에는 무시됨)
    /// </summary>
    public void SetMoveDirection(Vector2 dir)
    {
        if (isDashing || isKnockbacked) return;
        moveInput = dir;
    }

    /// <summary>
    /// 대시 시작
    /// </summary>
    public void StartDash(Vector2 dir)
    {
        if (dir == Vector2.zero || isKnockbacked) return;

        isDashing = true;
        dashDirection = dir.normalized;
        dashTimer = dashDuration;
    }

    /// <summary>
    /// 즉시 정지
    /// </summary>
    public void StopImmediately()
    {
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        isKnockbacked = false;
    }

    /// <summary>
    /// 외부에서 넉백 적용 (넉백 중 이동/대시 차단)
    /// </summary>
    public void ApplyKnockback(Vector2 force)
    {
        isKnockbacked = true;
        knockbackTimer = knockbackDuration;

        rb.linearVelocity = Vector2.zero; // 기존 속도 제거
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
