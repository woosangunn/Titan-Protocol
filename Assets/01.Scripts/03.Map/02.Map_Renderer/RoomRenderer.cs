using UnityEngine;
using MyGame.Map;

/// <summary>
/// 방 렌더링 컨트롤러: 타일 렌더링과 문 렌더링을 통합적으로 관리하는 클래스
/// </summary>
public class RoomRenderer : MonoBehaviour
{
    public RoomTileRenderer tileRenderer;   // 바닥 및 벽 타일을 그리는 컴포넌트
    public DoorRenderer doorRenderer;       // 문 타일 및 프리팹을 렌더링하는 컴포넌트

    /// <summary>
    /// 방 전체를 렌더링 (바닥, 벽, 문 포함)
    /// </summary>
    /// <param name="room">렌더링할 방 데이터</param>
    /// <param name="origin">타일맵 상에서 방이 시작되는 기준 위치</param>
    /// <param name="loader">문 이동 시 사용할 RoomLoader 참조</param>
    public void Render(RoomData room, Vector3Int origin, RoomLoader loader)
    {
        // 방 타일 그리기 (바닥, 벽)
        tileRenderer.DrawTiles(room, origin);

        // 문 타일 + 문 프리팹 생성
        doorRenderer.SpawnDoors(room, origin, loader);
    }

    /// <summary>
    /// 현재 방에 있는 모든 문을 여는 요청 전달
    /// </summary>
    public void OpenAllDoors()
    {
        doorRenderer.OpenAllDoors();
    }
}
