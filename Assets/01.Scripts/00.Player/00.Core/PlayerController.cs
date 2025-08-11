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

        if (input.RightClick && !grapple.IsAttached)
            attack.ShootChain(input.MouseWorldPos2D);

        if (input.LeftClick)
            attack.ShootBullet(input.MouseWorldPos2D);

        if (input.DashPressed)
        {
            Vector2 dashDir = input.LastMove;

            if (grapple.IsAttached)
            {
                if (dashDir.y > 0)
                    grapple.DoGrappleDash();
                else if (dashDir.x != 0)
                    grapple.DoSwing(dashDir.x);
            }
            else
            {
                // movement.StartDash(dashDir);
            }
        }
    }

    void FixedUpdate()
    {
        if (grapple.IsAttached) return;

        movement.SetMoveDirection(input.Move);
    }
}
