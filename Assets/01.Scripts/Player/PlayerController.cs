using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    public float grappleSpeed = 25f;
    private bool isGrappling = false;
    private Vector2 grappleTarget;

    private PlayerInput input;
    private PlayerMovement movement;
    private PlayerAttack attack;
    private Rigidbody2D rb;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.HandleInput();

        if (input.DashPressed && !isGrappling)
            movement.StartDash(input.Move);

        if (input.LeftClick)
            attack.ShootBullet(input.MouseWorldPos2D);

        if (input.RightClick && !isGrappling)
            attack.ShootChain(input.MouseWorldPos2D);
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            Vector2 toTarget = grappleTarget - (Vector2)transform.position;
            if (toTarget.magnitude < 0.3f)
            {
                rb.linearVelocity = Vector2.zero;
                isGrappling = false;
            }
            else
            {
                rb.linearVelocity = toTarget.normalized * grappleSpeed;
            }
        }
        else
        {
            movement.SetMoveDirection(input.Move);
        }
    }

    public void StartGrapple(Vector2 targetPos)
    {
        grappleTarget = targetPos;
        isGrappling = true;
        movement.StopImmediately(); // 대쉬 중이라면 중단
    }
}
