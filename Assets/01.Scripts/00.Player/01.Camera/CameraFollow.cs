using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // 따라갈 대상 (플레이어 등)
    public float smoothSpeed = 5f;     // 카메라 이동 부드러움 속도
    public Vector2 offset;             // 대상과 카메라 간 위치 오프셋

    void LateUpdate()
    {
        if (target == null) return;   // 대상이 없으면 동작하지 않음

        // 대상 위치에 오프셋을 더한 목표 위치 계산
        Vector3 desiredPosition = target.position + (Vector3)offset;

        // 현재 위치에서 목표 위치로 부드럽게 이동 (z축은 카메라 고정 -10)
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(desiredPosition.x, desiredPosition.y, -10f),
            smoothSpeed * Time.deltaTime);
    }
}
