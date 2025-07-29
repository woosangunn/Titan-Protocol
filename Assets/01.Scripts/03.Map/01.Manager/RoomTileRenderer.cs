using UnityEngine;
using UnityEngine.Tilemaps;
using MyGame.Map;

public class RoomTileRenderer : MonoBehaviour
{
    public Tilemap floorTilemap;   // 바닥 전용 타일맵
    public Tilemap wallTilemap;    // 벽과 문 전용 타일맵

    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;

    /// <summary>
    /// 방 타일 그리기
    /// </summary>
    public void DrawRoom(RoomData room, Vector3Int offset)
    {
        Vector2 size = room.size;

        // 1. 바닥 타일 그리기 (콜라이더 없음)
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = offset + new Vector3Int(x, y, 0);
                floorTilemap.SetTile(pos, floorTile);
            }
        }

        // 2. 벽 및 문 타일 그리기 (콜라이더 있음)
        int width = (int)size.x;
        int height = (int)size.y;

        // 벽: 방 외곽 (사방)
        for (int x = 0; x < width; x++)
        {
            Vector3Int bottomPos = offset + new Vector3Int(x, 0, 0);
            Vector3Int topPos = offset + new Vector3Int(x, height - 1, 0);

            wallTilemap.SetTile(bottomPos, wallTile);
            wallTilemap.SetTile(topPos, wallTile);
        }
        for (int y = 1; y < height - 1; y++)
        {
            Vector3Int leftPos = offset + new Vector3Int(0, y, 0);
            Vector3Int rightPos = offset + new Vector3Int(width - 1, y, 0);

            wallTilemap.SetTile(leftPos, wallTile);
            wallTilemap.SetTile(rightPos, wallTile);
        }

        // 3. 문 타일 그리기 (방의 neighborDirs 정보 활용)
        foreach (var dir in room.neighborDirs)
        {
            Vector3Int doorPos = offset;

            if (dir == Vector2Int.up)
            {
                doorPos += new Vector3Int(width / 2, height - 1, 0);
            }
            else if (dir == Vector2Int.down)
            {
                doorPos += new Vector3Int(width / 2, 0, 0);
            }
            else if (dir == Vector2Int.left)
            {
                doorPos += new Vector3Int(0, height / 2, 0);
            }
            else if (dir == Vector2Int.right)
            {
                doorPos += new Vector3Int(width - 1, height / 2, 0);
            }

            wallTilemap.SetTile(doorPos, doorTile);
        }
    }

    /// <summary>
    /// 방 타일 클리어 (바닥+벽)
    /// </summary>
    public void ClearRoom(Vector3Int offset, Vector2 size)
    {
        for (int x = 0; x < (int)size.x; x++)
        {
            for (int y = 0; y < (int)size.y; y++)
            {
                Vector3Int pos = offset + new Vector3Int(x, y, 0);
                floorTilemap.SetTile(pos, null);
                wallTilemap.SetTile(pos, null);
            }
        }
    }
}
