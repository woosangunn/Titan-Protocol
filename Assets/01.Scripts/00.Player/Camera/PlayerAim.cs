using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Transform[] aimPivots; // 조준할 회전 대상 오브젝트들 (총구, 무기 등)
    Camera cam;

    void Start()
    {
        cam = Camera.main; // 메인 카메라 참조
    }

    void Update()
    {
        // 마우스의 월드 좌표 구하기
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // 모든 조준 대상에 대해 반복
        foreach (Transform pivot in aimPivots)
        {
            if (pivot == null) continue; // 널 체크

            // pivot 위치에서 마우스 방향으로 단위 벡터 계산
            Vector2 dir = (mouseWorldPos - (Vector2)pivot.position).normalized;

            // 방향 벡터를 각도로 변환 (라디안 -> 도)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // pivot 회전 적용 (Z축 기준 2D 회전)
            pivot.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
