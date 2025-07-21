using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // 플레이어의 현재 이동 방향 (정규화된 벡터)
    public Vector2 Move { get; private set; }

    // 마지막으로 입력된 이동 방향 (이동 입력이 없을 때도 기억됨)
    public Vector2 LastMove { get; private set; }

    // 대시 입력 여부 (스페이스바가 눌렸을 때 true)
    public bool DashPressed { get; private set; }

    // 왼쪽 마우스 클릭 여부 (클릭이 발생한 프레임에 true)
    public bool LeftClick { get; private set; }

    // 오른쪽 마우스 클릭 여부 (클릭이 발생한 프레임에 true)
    public bool RightClick { get; private set; }

    // 현재 마우스 커서의 월드 좌표 (2D 평면 기준)
    public Vector2 MouseWorldPos2D => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    // 매 프레임 호출되어 플레이어 입력 상태를 갱신
    public void HandleInput()
    {
        float h = 0f, v = 0f;

        // 좌우 이동 입력 체크 (A, D 키)
        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;

        // 상하 이동 입력 체크 (W, S 키)
        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;

        // 입력 벡터 생성 및 정규화 (대각선 이동 시 속도 균일화)
        Vector2 input = new Vector2(h, v);
        Move = input.normalized;

        // 이동 입력이 있을 경우 마지막 이동 방향 갱신
        if (input != Vector2.zero)
            LastMove = Move;

        // 스페이스바 눌림 여부 체크 (한 프레임만 true)
        DashPressed = Input.GetKeyDown(KeyCode.Space);

        // 마우스 왼쪽 버튼 클릭 체크 (한 프레임만 true)
        LeftClick = Input.GetMouseButtonDown(0);

        // 마우스 오른쪽 버튼 클릭 체크 (한 프레임만 true)
        RightClick = Input.GetMouseButtonDown(1);
    }
}
