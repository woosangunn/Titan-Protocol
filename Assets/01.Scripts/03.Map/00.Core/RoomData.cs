using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Map
{
    public class RoomData
    {
        public RoomType type;
        public RoomState state = RoomState.Unvisited;

        public Vector2Int cellPosition;
        public Vector2Int cellSize;
        public Vector2Int tileSize;

        public List<DoorData> doors = new();
        public List<Vector2> enemySpawnPoints = new();
        public List<Vector2Int> obstaclePositions = new();

        public Vector2 GetTileCenterWorldPos() => new Vector2(tileSize.x, tileSize.y) * 0.5f;
    }
}
