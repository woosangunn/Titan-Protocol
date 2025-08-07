using UnityEngine;
using UnityEngine.Tilemaps;
using MyGame.Map;

/// <summary>
/// Ÿ�ϸ� ������� ���� �ٴ�, ��, ��ֹ� ���� �׸��� Ŭ����.
/// </summary>
public class RoomTileRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;   // �ٴ� Ÿ�ϸ�
    public Tilemap wallTilemap;    // �� �� ��ֹ� Ÿ�ϸ�

    [Header("Tiles")]
    public TileBase floorTile;     // �ٴ� Ÿ��
    public TileBase wallTile;      // �� Ÿ��
    public TileBase obstacleTile;  // ��ֹ� Ÿ��

    /// <summary>
    /// ���� ��ü Ÿ�� (�ٴ�, ��, ��ֹ�)�� �������Ѵ�.
    /// </summary>
    public void DrawTiles(RoomData room, Vector3Int origin)
    {
        // ������ �׷��� Ÿ�� ��� ����
        ClearTiles();

        // �ٴ� + �� Ÿ�� �׸���
        DrawFloorAndWalls(room, origin);

        // ��ֹ� Ÿ�� �׸���
        DrawObstacles(room, origin);
    }

    /// <summary>
    /// �ٴ� �� �� Ÿ�ϸ��� �ʱ�ȭ (��� Ÿ�� ����)
    /// </summary>
    private void ClearTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    /// <summary>
    /// ���� �ٴڰ� �׵θ� �� Ÿ���� �׸�
    /// </summary>
    private void DrawFloorAndWalls(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                // ���� Ÿ�� ��ǥ = �� ���� + (x, y)
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);

                // �ٴ� Ÿ���� �׻� �׸�
                floorTilemap.SetTile(tilePos, floorTile);

                // �׵θ��� ��� �� Ÿ���� �߰��� �׸�
                if (IsBorder(x, y, room.tileSize))
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }
    }

    /// <summary>
    /// �� ������ ��ֹ� ��ǥ�� ��ֹ� Ÿ���� �׸�
    /// </summary>
    private void DrawObstacles(RoomData room, Vector3Int origin)
    {
        foreach (var localPos in room.obstaclePositions)
        {
            // ���� ��ǥ�� ���� ��ǥ�� ��ȯ
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            // �ش� ��ġ�� ��ֹ� Ÿ�� ����
            wallTilemap.SetTile(tilePos, obstacleTile);
        }
    }

    /// <summary>
    /// �־��� ��ǥ�� ���� �׵θ����� ���� Ȯ��
    /// </summary>
    private bool IsBorder(int x, int y, Vector2Int size)
    {
        return x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
    }
}
