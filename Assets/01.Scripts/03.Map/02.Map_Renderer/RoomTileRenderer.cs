using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using MyGame.Map;

public class RoomTileRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;       // 방 바닥을 표시할 Tilemap
    public Tilemap wallTilemap;        // 방 벽(또는 장애물)을 표시할 Tilemap

    [Header("Tiles")]
    public TileBase floorTile;         // 바닥에 사용할 타일
    public TileBase wallTile;          // 벽에 사용할 타일
    public TileBase doorTile;          // 문에 사용할 타일 (문이 타일 형태로 표현될 때 사용)

    [Header("Prefabs")]
    public GameObject doorPrefab;      // 문 오브젝트 프리팹 (문 트리거 포함)

    private readonly List<GameObject> spawnedDoors = new(); // 생성된 문 오브젝트들을 추적하는 리스트

    // 현재 방의 타일 및 문 오브젝트 제거
    public void ClearTiles()
    {
        // 바닥 및 벽 타일 삭제
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // 생성된 문 오브젝트 제거
        foreach (var door in spawnedDoors)
        {
            if (door != null)
                Destroy(door);
        }
        spawnedDoors.Clear(); // 리스트 초기화
    }

    // 방을 구성하는 모든 요소를 그리는 함수
    public void DrawRoom(RoomData room, Vector3Int origin)
    {
        DrawFloorAndWalls(room, origin); // 바닥 및 벽 타일
        DrawDoors(room, origin);         // 문 타일 + 프리팹
        DrawObstacles(room, origin);     // 기둥 등 장애물
        // 향후 다른 장식 추가 예정 (TODO)
    }

    // 방 내부 바닥 및 외곽 벽 그리기
    private void DrawFloorAndWalls(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);

                // 바닥 타일 설정
                floorTilemap.SetTile(tilePos, floorTile);

                // 테두리라면 벽 타일로 덮기
                if (IsBorder(x, y, room.tileSize))
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }
    }

    // 문 생성 (타일 + 프리팹)
    private void DrawDoors(RoomData room, Vector3Int origin)
    {
        var roomLoader = FindAnyObjectByType<RoomLoader>(); // 문 동작용 RoomLoader 탐색
        if (roomLoader == null || doorPrefab == null)
            return;

        foreach (Vector2Int localPos in room.doorLocalPositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // 문 타일 설정 (기존 벽을 문으로 대체)
            if (doorTile != null)
                wallTilemap.SetTile(tilePos, doorTile);

            // 문 프리팹 배치 (위치 보정 포함)
            Vector3 worldPos = wallTilemap.CellToWorld(tilePos) + new Vector3(0, 0, 0); // 정중앙 위치

            // 문 프리팹 생성 및 방향 설정
            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);
            if (door.TryGetComponent(out DoorTrigger trigger))
            {
                trigger.direction = GetDirectionFromDoor(localPos, room.tileSize); // 문 방향 설정
                trigger.roomLoader = roomLoader; // 현재 룸로더 전달
            }

            spawnedDoors.Add(door); // 추적용 리스트에 등록
        }
    }

    // 장애물(기둥 등)을 그리는 함수
    private void DrawObstacles(RoomData room, Vector3Int origin)
    {
        if (room.obstaclePositions == null || room.obstaclePositions.Count == 0)
            return;

        foreach (var localPos in room.obstaclePositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // 장애물은 벽 타일을 재활용 (구분이 필요하면 별도 타일 필요)
            wallTilemap.SetTile(tilePos, wallTile);
        }
    }

    // 해당 위치가 방의 테두리인지 확인 (0이나 끝 좌표)
    private bool IsBorder(int x, int y, Vector2Int size)
    {
        return x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
    }

    // 문 위치(localPos) 기준으로 연결 방향 계산
    private Vector2Int GetDirectionFromDoor(Vector2Int localPos, Vector2Int tileSize)
    {
        if (localPos.x == 0) return Vector2Int.left;
        if (localPos.x == tileSize.x - 1) return Vector2Int.right;
        if (localPos.y == 0) return Vector2Int.down;
        if (localPos.y == tileSize.y - 1) return Vector2Int.up;
        return Vector2Int.zero; // 예외 처리 (방 내부에 문이 있다면 방향 없음)
    }

    // 모든 문 오브젝트를 열도록 명령
    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
        {
            if (door != null && door.TryGetComponent(out DoorTrigger trigger))
                trigger.Open();
        }
    }
}
