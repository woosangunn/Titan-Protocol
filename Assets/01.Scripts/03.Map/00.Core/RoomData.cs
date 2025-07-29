using UnityEngine;
using System.Collections.Generic;

namespace MyGame.Map
{
    /// <summary>
    /// ���� Ÿ���� ��Ÿ���ϴ�.
    /// </summary>
    public enum RoomType
    {
        Start,      // ���� ��
        Combat,     // ���� ��
        Shop,       // ���� ��
        Item,       // ������ ��
        Boss        // ���� ��
    }

    /// <summary>
    /// ���� ���� ���¸� ��Ÿ���ϴ�.
    /// </summary>
    public enum RoomState
    {
        Unvisited,      // �湮���� ����
        Discovered,     // �湮������ Ŭ�������� ����
        Cleared         // Ŭ����� ��
    }

    /// <summary>
    /// ���� �� �ϳ��� ǥ���ϴ� ������ Ŭ�����Դϴ�.
    /// </summary>
    public class RoomData
    {
        public Vector2Int position;              // �� ��ġ (�ʻ��� ��ǥ)
        public RoomType type;                    // �� ����
        public RoomState state = RoomState.Unvisited; // �ʱ� ����
        public Vector2 size;                     // �� ũ�� (Ÿ�� ����)
        public List<Vector2Int> neighborDirs = new(); // ������ ���� ���� (ex. Vector2Int.up)
    }
}
