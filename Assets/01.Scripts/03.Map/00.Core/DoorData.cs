using UnityEngine;

namespace MyGame.Map
{
    public class DoorData
    {
        public Vector2Int localPosition;         // ���� �濡���� �� ��ġ
        public Vector2Int direction;             // ���� ���ϴ� ���� (ex: Vector2Int.up)
        public Vector2Int connectedRoomPos;      // ����� ���� �� ��ǥ
    }
}