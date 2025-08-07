using UnityEngine;

namespace MyGame.Map
{
    public class DoorData
    {
        public Vector2Int localPosition;         // 현재 방에서의 문 위치
        public Vector2Int direction;             // 문이 향하는 방향 (ex: Vector2Int.up)
        public Vector2Int connectedRoomPos;      // 연결된 방의 셀 좌표
    }
}