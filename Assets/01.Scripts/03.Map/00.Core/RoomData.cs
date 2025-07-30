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

        // ���� �߰��� �� ��ġ (Ÿ�ϸ� �� ���� ��ǥ ����)
        public List<Vector2Int> doorLocalPositions = new();
    }
}
