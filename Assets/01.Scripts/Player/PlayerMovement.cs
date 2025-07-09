// PlayerMovement.cs (Grapple 상태 시 물리 덮어쓰기 방지)
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private Vector2 dashDirection;
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
        // 사슬 연결 중이면 물리 이동 중단 (Grapple에서 직접 제어)
        if (grapple != null && grapple.IsAttached) return;

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

    public void SetMoveDirection(Vector2 dir)
    {
        if (!isDashing)
            moveInput = dir;
    }

    public void StartDash(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        isDashing = true;
        dashDirection = dir.normalized;
        dashTimer = dashDuration;
    }

    public void StopImmediately()
    {
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}
