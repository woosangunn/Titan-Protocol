using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerGrappleHandler grapple;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        grapple = GetComponent<PlayerGrappleHandler>();
    }

    void Update()
    {
        input.HandleInput();

        // 사슬 발사 (붙어있지 않을 때만)
        if (input.RightClick && !grapple.IsAttached)
            attack.ShootChain(input.MouseWorldPos2D);

        // 총 발사
        if (input.LeftClick)
            attack.ShootBullet(input.MouseWorldPos2D);

        // Space 입력 처리
        if (input.DashPressed)
        {
            Vector2 dashDir = input.LastMove;

            if (grapple.IsAttached)
            {
                // W + Space → 그랩 대시
                if (dashDir.y > 0)
                    grapple.DoGrappleDash();
                // A/D + Space → 스윙
                else if (dashDir.x != 0)
                    grapple.DoSwing(dashDir.x);
            }
            else
            {
                // 일반 대시 비활성화
                // movement.StartDash(dashDir); // <- 필요시 다시 활성화
            }
        }
    }

    void FixedUpdate()
    {
        if (grapple.IsAttached) return;
        movement.SetMoveDirection(input.Move);
    }
}
