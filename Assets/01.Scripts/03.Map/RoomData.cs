using UnityEngine;
using System.Collections.Generic;

public class RoomData
{
    public Vector2Int position;         // 그리드상의 위치
    public RoomType type;              // 방의 유형
    public RoomState state = RoomState.Unvisited;

    public GameObject instance = null; // 실제 씬에 생성된 프리팹
    public List<Vector2Int> neighborDirs = new(); // 연결된 방향들
}
