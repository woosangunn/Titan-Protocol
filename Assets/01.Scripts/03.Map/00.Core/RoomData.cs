using UnityEngine;
using System.Collections.Generic;

namespace MyGame.Map
{
    /// <summary>
    /// 방의 타입을 나타냅니다.
    /// </summary>
    public enum RoomType
    {
        Start,      // 시작 방
        Combat,     // 전투 방
        Shop,       // 상점 방
        Item,       // 아이템 방
        Boss        // 보스 방
    }

    /// <summary>
    /// 방의 현재 상태를 나타냅니다.
    /// </summary>
    public enum RoomState
    {
        Unvisited,      // 방문하지 않음
        Discovered,     // 방문했지만 클리어하지 않음
        Cleared         // 클리어된 방
    }

    /// <summary>
    /// 맵의 방 하나를 표현하는 데이터 클래스입니다.
    /// </summary>
    public class RoomData
    {
        public Vector2Int position;              // 방 위치 (맵상의 좌표)
        public RoomType type;                    // 방 종류
        public RoomState state = RoomState.Unvisited; // 초기 상태
        public Vector2 size;                     // 방 크기 (타일 단위)
        public List<Vector2Int> neighborDirs = new(); // 인접한 방향 정보 (ex. Vector2Int.up)
    }
}
