using UnityEngine;
using System.Collections.Generic;

namespace MyGame.Map
{
    public enum RoomType { Start, Combat, Shop, Item, Boss }
    public enum RoomState { Unvisited, Discovered, Cleared }

    public class RoomData
    {
        public Vector2Int cellPosition;     // 맵 그리드 상 위치
        public Vector2Int cellSize;         // 셀 단위 방 크기 (ex: 2x2 셀)
        public Vector2Int tileSize;         // 타일 단위 방 크기 (cellSize * tilePerCell)
        public RoomType type;
        public RoomState state = RoomState.Unvisited;

        public List<Vector2Int> neighborDirs = new();
        public List<Vector2Int> doorLocalPositions = new();
    }
}
