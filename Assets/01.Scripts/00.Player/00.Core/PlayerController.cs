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
        // Rigidbody2D 컴포넌트 가져오기 및 초기 설정
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;                           // 중력 효과 제거 (2D 플랫한 움직임 위해)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // 회전 고정

        // 플레이어 관련 다른 컴포넌트들 가져오기
        input = GetComponent<PlayerInput>();          // 입력 처리 컴포넌트
        movement = GetComponent<PlayerMovement>();    // 이동 처리 컴포넌트
        attack = GetComponent<PlayerAttack>();        // 공격 처리 컴포넌트
        grapple = GetComponent<PlayerGrappleHandler>();// 그랩(사슬) 처리 컴포넌트
    }

    void Update()
    {
        // 입력을 매 프레임마다 처리
        input.HandleInput();

        // 마우스 오른쪽 클릭 + 사슬이 아직 붙어있지 않을 때 사슬 발사
        if (input.RightClick && !grapple.IsAttached)
            attack.ShootChain(input.MouseWorldPos2D);

        // 마우스 왼쪽 클릭 시 총알 발사
        if (input.LeftClick)
            attack.ShootBullet(input.MouseWorldPos2D);

        // 스페이스바 입력 처리
        if (input.DashPressed)
        {
            Vector2 dashDir = input.LastMove;  // 마지막 이동 방향 가져오기

            if (grapple.IsAttached)
            {
                // 그랩 상태일 때 방향에 따른 행동 분기

                // 위쪽 방향키 + 스페이스 → 그랩 대시 실행
                if (dashDir.y > 0)
                    grapple.DoGrappleDash();

                // 좌우 방향키 + 스페이스 → 스윙 동작 실행
                else if (dashDir.x != 0)
                    grapple.DoSwing(dashDir.x);
            }
            else
            {
                // 그랩이 붙어있지 않을 때는 일반 대시 비활성화 상태
                // 필요하면 아래 주석 해제 후 일반 대시 활성화 가능
                // movement.StartDash(dashDir);
            }
        }
    }

    void FixedUpdate()
    {
        // 물리 업데이트 시, 그랩에 붙어있지 않을 때만 이동 방향 설정
        if (grapple.IsAttached) return;

        movement.SetMoveDirection(input.Move);
    }
}
