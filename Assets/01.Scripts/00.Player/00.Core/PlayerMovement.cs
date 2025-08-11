using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

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
        if (grapple != null && grapple.IsAttached) return;

        if (isKnockbacked)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
                isKnockbacked = false;

            return;
        }

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
        if (isDashing || isKnockbacked) return;
        moveInput = dir;
    }

    public void StartDash(Vector2 dir)
    {
        if (dir == Vector2.zero || isKnockbacked) return;

        isDashing = true;
        dashDirection = dir.normalized;
        dashTimer = dashDuration;
    }

    public void StopImmediately()
    {
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        isKnockbacked = false;
    }

    public void ApplyKnockback(Vector2 force)
    {
        isKnockbacked = true;
        knockbackTimer = knockbackDuration;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
