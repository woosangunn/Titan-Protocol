using UnityEngine;
using System.Collections.Generic;

namespace MyGame.Map
{
    public enum RoomType
    {
        Start,
        Combat,
        Shop,
        Item,
        Boss
    }

    public enum RoomState
    {
        Unvisited,
        Discovered,
        Cleared
    }

    public class RoomData
    {
        public Vector2Int position;
        public RoomType type;
        public RoomState state = RoomState.Unvisited;
        public Vector2Int size;
        public List<Vector2Int> neighborDirs = new();

        // 새로 추가된 문 위치 (타일맵 내 로컬 좌표 기준)
        public List<Vector2Int> doorLocalPositions = new();
    }
}
