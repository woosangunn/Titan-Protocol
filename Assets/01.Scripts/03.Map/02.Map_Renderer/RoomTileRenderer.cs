using UnityEngine;
using UnityEngine.Tilemaps;
using MyGame.Map;

public class RoomTileRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap; 
    public Tilemap wallTilemap;

    [Header("Tiles")]
    public TileBase[] floorTile; 
    public TileBase wallTile; 
    public TileBase obstacleTile; 

    public void DrawTiles(RoomData room, Vector3Int origin)
    {
      
        ClearTiles();

        DrawFloorAndWalls(room, origin);

        DrawObstacles(room, origin);
    }

    private void ClearTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    private void DrawFloorAndWalls(RoomData room, Vector3Int origin)
    {
        for (int x = 0; x < room.tileSize.x; x++)
        {
            for (int y = 0; y < room.tileSize.y; y++)
            {
                Vector3Int tilePos = origin + new Vector3Int(x, y, 0);

                TileBase randomTile = floorTile[Random.Range(0, floorTile.Length)];
                floorTilemap.SetTile(tilePos, randomTile);

                if (IsBorder(x, y, room.tileSize))
                    wallTilemap.SetTile(tilePos, wallTile);
            }
        }
    }

    private void DrawObstacles(RoomData room, Vector3Int origin)
    {
        foreach (var localPos in room.obstaclePositions)
        {
            Vector3Int tilePos = origin + (Vector3Int)localPos;

            wallTilemap.SetTile(tilePos, obstacleTile);
        }
    }

    private bool IsBorder(int x, int y, Vector2Int size)
    {
        return x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1;
    }
}
