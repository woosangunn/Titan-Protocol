
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

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

        // 사슬 발사 조건: 연결되지 않았을 때만 발사 허용
        if (input.RightClick && !grapple.IsGrappleActive())
        {
            attack.ShootChain(input.MouseWorldPos2D);
        }

        if (input.LeftClick)
        {
            attack.ShootBullet(input.MouseWorldPos2D);
        }

        if (input.DashPressed)
        {
            if (grapple.IsGrappleActive())
            {
                if (input.Move.y > 0)
                    grapple.DoGrappleDash();
                else if (input.Move.x != 0)
                    grapple.DoSwing(input.Move.x);
            }
            else
            {
                movement.StartDash(input.Move);
            }
        }
    }

    void FixedUpdate()
    {
        if (grapple.IsGrappleActive()) return;
        movement.SetMoveDirection(input.Move);
    }
}
