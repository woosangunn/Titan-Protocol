using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using MyGame.Map;

public class RoomTileRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [Header("Tiles")]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;

    [Header("Prefabs")]
    public GameObject doorPrefab;

    private readonly List<GameObject> spawnedDoors = new();

    // 방 타일 모두 삭제 및 생성된 문 삭제
    public void ClearTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        foreach (var door in spawnedDoors)
        {
            if (door != null)
                Destroy(door);
        }
        spawnedDoors.Clear();
    }

    // 방 그리기 함수
    public void DrawRoom(RoomData room, Vector3Int origin)
    {
        DrawFloorAndWalls(room, origin);
        DrawDoors(room, origin);
        DrawObstacles(room, origin);  // 장애물(기둥) 그리기 추가
        // TODO: 추가 장애물이나 장식물 슬롯
    }

    // 바닥과 벽 타일 그리기
    private void DrawFloorAndWalls(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);
                floorTilemap.SetTile(tilePos, floorTile);

                // 방 테두리는 벽 타일로 처리
                if (IsBorder(x, y, room.tileSize))
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }
    }

    // 문 타일과 문 프리팹 생성
    private void DrawDoors(RoomData room, Vector3Int origin)
    {
        var roomLoader = FindAnyObjectByType<RoomLoader>();
        if (roomLoader == null || doorPrefab == null)
            return;

        foreach (Vector2Int localPos in room.doorLocalPositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // 벽 타일을 문 타일로 대체
            if (doorTile != null)
                wallTilemap.SetTile(tilePos, doorTile);

            // 문 프리팹 위치 보정 (타일 중심)
            Vector3 worldPos = wallTilemap.CellToWorld(tilePos) + new Vector3(0, 0, 0);

            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);
            if (door.TryGetComponent(out DoorTrigger trigger))
            {
                trigger.direction = GetDirectionFromDoor(localPos, room.tileSize);
                trigger.roomLoader = roomLoader;
            }

            spawnedDoors.Add(door);
        }
    }

    // 장애물(기둥) 타일 그리기
    private void DrawObstacles(RoomData room, Vector3Int origin)
    {
        if (room.obstaclePositions == null || room.obstaclePositions.Count == 0)
            return;

        foreach (var localPos in room.obstaclePositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // 장애물은 벽 타일로 표시 (기둥 역할)
            wallTilemap.SetTile(tilePos, wallTile);
        }
    }

    private bool IsBorder(int x, int y, Vector2Int size)
    {
        return x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
    }

    private Vector2Int GetDirectionFromDoor(Vector2Int localPos, Vector2Int tileSize)
    {
        if (localPos.x == 0) return Vector2Int.left;
        if (localPos.x == tileSize.x - 1) return Vector2Int.right;
        if (localPos.y == 0) return Vector2Int.down;
        if (localPos.y == tileSize.y - 1) return Vector2Int.up;
        return Vector2Int.zero;
    }

    // 모든 문 열기 호출
    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
        {
            if (door != null && door.TryGetComponent(out DoorTrigger trigger))
                trigger.Open();
        }
    }
}
