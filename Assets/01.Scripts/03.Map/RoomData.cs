using UnityEngine;
using System.Collections.Generic;

public class RoomData
{
    public Vector2Int position;         // �׸������ ��ġ
    public RoomType type;              // ���� ����
    public RoomState state = RoomState.Unvisited;

    public GameObject instance = null; // ���� ���� ������ ������
    public List<Vector2Int> neighborDirs = new(); // ����� �����
}
