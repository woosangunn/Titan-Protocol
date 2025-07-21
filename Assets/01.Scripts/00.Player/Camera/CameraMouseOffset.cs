using UnityEngine;

public class CameraMouseOffset : MonoBehaviour
{
    public Transform target;        // 따라갈 대상 (플레이어 등)
    public float followSpeed = 5f;  // 카메라 이동 부드러움 속도
    public float maxOffset = 5f;    // 마우스 위치에 따른 최대 오프셋 거리

    Camera cam;

    void Start()
    {
        cam = Camera.main;          // 메인 카메라 참조
    }

    void LateUpdate()
    {
        if (!target) return;        // 대상이 없으면 실행하지 않음

        // 마우스 화면 좌표 → 월드 좌표 변환
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = target.position;

        // 대상 위치와 마우스 위치 간 방향 및 거리 계산
        Vector2 offsetDir = mouseWorldPos - targetPos;

        // 오프셋이 maxOffset보다 크면 최대값으로 제한
        if (offsetDir.magnitude > maxOffset)
            offsetDir = offsetDir.normalized * maxOffset;

        // 대상 위치에 제한된 오프셋 더하기
        Vector3 desiredPos = targetPos + (Vector3)offsetDir;

        // 카메라 위치를 부드럽게 목표 위치로 이동 (z축 고정 -10)
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(desiredPos.x, desiredPos.y, -10f),
            followSpeed * Time.deltaTime);
    }
}
