using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;

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
            Debug.Log($"[DashPressed] Move: {dashDir} | y:{dashDir.y} x:{dashDir.x}");

            if (grapple.IsAttached)
            {
                if (dashDir.y > 0)
                {
                    Debug.Log("▶ W + Space → Grapple Dash");
                    grapple.DoGrappleDash();
                }
                else if (dashDir.x != 0)
                {
                    Debug.Log("▶ A/D + Space → Grapple Swing");
                    grapple.DoSwing(dashDir.x);
                }
            }
            else
            {
                Debug.Log("▶ 일반 Dash");
                movement.StartDash(dashDir);
            }
        }
    }

    void FixedUpdate()
    {
        if (grapple.IsAttached) return;
        movement.SetMoveDirection(input.Move);
    }
}
