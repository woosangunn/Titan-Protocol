using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동/대시")]
    [Tooltip("플레이어 이동 속도")]
    public float moveSpeed = 5f;
    [Tooltip("대시 속도")]
    public float dashSpeed = 15f;
    [Tooltip("대시 지속 시간")]
    public float dashDuration = 0.2f;

    [Header("넉백")]
    [Tooltip("넉백 지속 시간")]
    public float knockbackDuration = 0.2f;


    private Rigidbody2D rb;                // 물리 이동을 위한 Rigidbody2D 컴포넌트
    private Vector2 moveInput;             // 플레이어가 입력한 이동 방향
    private Vector2 dashDirection;         // 대시 방향

    private bool isDashing = false;        // 대시 중 상태 여부
    private float dashTimer = 0f;          // 대시 시간 카운터

    private bool isKnockbacked = false;    // 넉백 중 상태 여부
    private float knockbackTimer = 0f;     // 넉백 시간 카운터

    private PlayerGrappleHandler grapple;  // 사슬 그랩 핸들러 (붙어있으면 이동 제어 제한)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;                           // 중력 제거
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 고정

        grapple = GetComponent<PlayerGrappleHandler>();
    }

    void FixedUpdate()
    {
        // 사슬에 붙어있으면 이동/대시 제어를 하지 않음
        if (grapple != null && grapple.IsAttached) return;

        // 넉백 상태일 때
        if (isKnockbacked)
        {
            knockbackTimer -= Time.fixedDeltaTime;   // 넉백 시간 감소
            if (knockbackTimer <= 0f)
                isKnockbacked = false;                // 넉백 종료

            return; // 넉백 중에는 이동이나 대시 무시
        }

        // 대시 중일 때
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;  // 대시 방향과 속도로 이동
            dashTimer -= Time.fixedDeltaTime;         // 대시 시간 감소
            if (dashTimer <= 0f)
                isDashing = false;                     // 대시 종료
        }
        else
        {
            // 일반 이동 처리
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    /// <summary>
    /// 이동 방향 설정 (대시나 넉백 중에는 입력 무시)
    /// </summary>
    public void SetMoveDirection(Vector2 dir)
    {
        if (isDashing || isKnockbacked) return;
        moveInput = dir;
    }

    /// <summary>
    /// 대시 시작 함수
    /// </summary>
    public void StartDash(Vector2 dir)
    {
        if (dir == Vector2.zero || isKnockbacked) return; // 넉백 중이거나 방향이 없으면 대시 안 함

        isDashing = true;                 // 대시 상태 활성화
        dashDirection = dir.normalized;  // 대시 방향 정규화
        dashTimer = dashDuration;        // 대시 시간 초기화
    }

    /// <summary>
    /// 즉시 이동 멈춤 (대시, 넉백 상태도 초기화)
    /// </summary>
    public void StopImmediately()
    {
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        isKnockbacked = false;
    }

    /// <summary>
    /// 넉백 효과 적용 (넉백 상태 시작, 힘을 가함)
    /// </summary>
    public void ApplyKnockback(Vector2 force)
    {
        isKnockbacked = true;
        knockbackTimer = knockbackDuration;

        rb.linearVelocity = Vector2.zero;          // 현재 속도 초기화
        rb.AddForce(force, ForceMode2D.Impulse); // 넉백 힘 즉시 가하기
    }
}
