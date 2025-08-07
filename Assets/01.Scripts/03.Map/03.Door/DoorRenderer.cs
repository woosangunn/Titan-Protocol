using MyGame.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 문 렌더링 및 문 프리팹 배치를 담당하는 컴포넌트
/// 문 타일, 문 오브젝트, 문 트리거를 생성 및 관리함
/// </summary>
public class DoorRenderer : MonoBehaviour
{
    public Tilemap wallTilemap;      // 문 타일을 그릴 벽 타일맵
    public TileBase doorTile;        // 문 타일로 사용할 타일
    public GameObject doorPrefab;    // 문 프리팹 (문 트리거 포함)

    private List<GameObject> spawnedDoors = new(); // 생성된 문 프리팹 리스트

    /// <summary>
    /// 전달받은 방(RoomData)의 문 정보를 기반으로 문 타일과 프리팹을 생성
    /// </summary>
    /// <param name="room">현재 방 데이터</param>
    /// <param name="origin">방의 원점 좌표 (타일맵 기준)</param>
    /// <param name="loader">방 이동을 담당하는 RoomLoader 참조</param>
    public void SpawnDoors(RoomData room, Vector3Int origin, RoomLoader loader)
    {
        ClearDoors(); // 기존 문 제거

        foreach (var doorData in room.doors)
        {
            // 방 원점 + 로컬 문 위치 = 타일맵 상 문 위치
            Vector3Int tilePos = origin + (Vector3Int)doorData.localPosition;

            // 타일로 문 그리기 (선택사항)
            if (doorTile != null)
                wallTilemap.SetTile(tilePos, doorTile);

            // 문 프리팹 생성 (타일맵 셀을 월드 좌표로 변환해서 위치 지정)
            Vector3 worldPos = wallTilemap.CellToWorld(tilePos);
            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);

            // DoorTrigger 설정 (방 이동 방향 및 로더 연결)
            if (door.TryGetComponent(out DoorTrigger trigger))
            {
                trigger.direction = doorData.direction;   // 문이 향한 방향 (예: 오른쪽이면 플레이어는 왼쪽으로 들어오게 됨)
                trigger.roomLoader = loader;

                // 전투방이 아닌 경우, 처음부터 문을 열어둠
                if (room.type != RoomType.Combat)
                    trigger.Open();
            }

            // 생성된 문을 리스트에 저장해 나중에 정리하거나 열 수 있게 함
            spawnedDoors.Add(door);
        }
    }

    /// <summary>
    /// 생성된 모든 문을 열도록 요청
    /// </summary>
    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
        {
            if (door.TryGetComponent(out DoorTrigger trigger))
                trigger.Open();
        }
    }

    /// <summary>
    /// 기존에 생성된 문 프리팹들을 전부 제거
    /// </summary>
    public void ClearDoors()
    {
        foreach (var door in spawnedDoors)
            if (door) Destroy(door);

        spawnedDoors.Clear();
    }
}
