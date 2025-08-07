using UnityEngine;
using UnityEngine.Tilemaps;
using MyGame.Map;

/// <summary>
/// 타일맵 기반으로 방의 바닥, 벽, 장애물 등을 그리는 클래스.
/// </summary>
public class RoomTileRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;   // 바닥 타일맵
    public Tilemap wallTilemap;    // 벽 및 장애물 타일맵

    [Header("Tiles")]
    public TileBase floorTile;     // 바닥 타일
    public TileBase wallTile;      // 벽 타일
    public TileBase obstacleTile;  // 장애물 타일

    /// <summary>
    /// 방의 전체 타일 (바닥, 벽, 장애물)을 렌더링한다.
    /// </summary>
    public void DrawTiles(RoomData room, Vector3Int origin)
    {
        // 이전에 그려진 타일 모두 제거
        ClearTiles();

        // 바닥 + 벽 타일 그리기
        DrawFloorAndWalls(room, origin);

        // 장애물 타일 그리기
        DrawObstacles(room, origin);
    }

    /// <summary>
    /// 바닥 및 벽 타일맵을 초기화 (모든 타일 제거)
    /// </summary>
    private void ClearTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    /// <summary>
    /// 방의 바닥과 테두리 벽 타일을 그림
    /// </summary>
    private void DrawFloorAndWalls(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                // 현재 타일 좌표 = 방 원점 + (x, y)
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);

                // 바닥 타일은 항상 그림
                floorTilemap.SetTile(tilePos, floorTile);

                // 테두리일 경우 벽 타일을 추가로 그림
                if (IsBorder(x, y, room.tileSize))
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }
    }

    /// <summary>
    /// 방 내부의 장애물 좌표에 장애물 타일을 그림
    /// </summary>
    private void DrawObstacles(RoomData room, Vector3Int origin)
    {
        foreach (var localPos in room.obstaclePositions)
        {
            // 로컬 좌표를 월드 좌표로 변환
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // 해당 위치에 장애물 타일 설정
            wallTilemap.SetTile(tilePos, obstacleTile);
        }
    }

    /// <summary>
    /// 주어진 좌표가 방의 테두리인지 여부 확인
    /// </summary>
    private bool IsBorder(int x, int y, Vector2Int size)
    {
        return x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
    }
}
