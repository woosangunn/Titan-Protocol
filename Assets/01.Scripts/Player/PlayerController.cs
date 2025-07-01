using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput input;
    private PlayerMovement movement;
    private PlayerAttack attack;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        input.HandleInput();

        // 마우스 방향으로 총구 회전
        RotateToMouse(attack.gunPivot, input.MouseWorldPos2D);
        RotateToMouse(attack.chainPivot, input.MouseWorldPos2D);

        if (input.DashPressed)
            movement.StartDash(input.Move);

        if (input.LeftClick)
            attack.ShootBullet(input.MouseWorldPos2D);

        if (input.RightClick)
            attack.ShootChain(input.MouseWorldPos2D);
    }

    void RotateToMouse(Transform pivot, Vector2 mousePos)
    {
        Vector2 dir = (mousePos - (Vector2)pivot.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        pivot.rotation = Quaternion.Euler(0, 0, angle);
    }


    void FixedUpdate()
    {
        movement.SetMoveDirection(input.Move);
    }
}
