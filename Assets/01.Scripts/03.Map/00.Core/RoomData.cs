using UnityEngine;
using System.Collections.Generic;

namespace MyGame.Map
{
    public enum RoomType { Start, Combat, Shop, Item, Boss }
    public enum RoomState { Unvisited, Discovered, Cleared }

    public class RoomData
    {
        public Vector2Int cellPosition;     // �� �׸��� �� ��ġ
        public Vector2Int cellSize;         // �� ���� �� ũ�� (ex: 2x2 ��)
        public Vector2Int tileSize;         // Ÿ�� ���� �� ũ�� (cellSize * tilePerCell)
        public RoomType type;
        public RoomState state = RoomState.Unvisited;

        public List<Vector2Int> neighborDirs = new();
        public List<Vector2Int> doorLocalPositions = new();
    }
}
