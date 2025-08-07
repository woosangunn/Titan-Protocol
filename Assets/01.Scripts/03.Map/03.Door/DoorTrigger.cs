using UnityEngine;
using MyGame.Map;

/// <summary>
/// 문 충돌 감지 및 상태 관리 클래스
/// - 플레이어가 닫힌 문에 부딪혀도 아무 일도 일어나지 않음
/// - 열린 문에 플레이어가 닿으면 RoomLoader를 통해 방 이동 시도
/// </summary>
public class DoorTrigger : MonoBehaviour
{
    public Vector2Int direction;       // 이 문이 향하고 있는 방향 (예: 오른쪽 문이면 (1, 0))
    public RoomLoader roomLoader;      // 방 이동을 처리하는 RoomLoader 참조

    private bool isOpen = false;       // 문이 열려 있는 상태인지 여부

    /// <summary>
    /// 외부에서 이 메서드를 호출하면 문이 열린 상태로 설정됨
    /// - 이후 충돌 시 방 이동 가능
    /// - 애니메이션, 사운드 등은 이 안에서 추가 구현 가능
    /// </summary>
    public void Open()
    {
        isOpen = true;
        // TODO: 애니메이션 또는 상태 이펙트 추가 가능
    }

    /// <summary>
    /// 충돌 이벤트 감지 (플레이어가 문에 닿았을 때 실행)
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DoorTrigger OnTriggerEnter2D by {other.name}");

        // 문이 닫혀 있으면 아무 것도 하지 않음
        if (!isOpen) return;

        // 충돌한 대상이 플레이어가 아니면 무시
        if (!other.CompareTag("Player")) return;

        // 플레이어가 열린 문에 닿았으므로 해당 방향으로 방 이동 시도
        roomLoader.TryMove(direction);
    }
}
