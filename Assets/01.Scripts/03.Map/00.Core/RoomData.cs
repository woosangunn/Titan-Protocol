
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Map
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum RoomType
    {
        Start,
        Combat,
        Item,
        Boss,
        Shop
    }

    public enum RoomState
    {
        Unvisited,
        Discovered,
        Cleared
    }

    public class RoomData
    {
        public Vector2Int cellPosition;
        public Vector2Int cellSize;
        public Vector2Int tileSize;
        public RoomType type;
        public RoomState state = RoomState.Unvisited;

        public List<Vector2Int> neighborDirs = new();
        public List<Vector2Int> doorLocalPositions = new();
        public List<Vector2Int> obstaclePositions = new();

        public Vector2 GetTileCenterWorldPos() => new Vector2(tileSize.x, tileSize.y) * 0.5f;
    }
}
