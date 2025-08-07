using UnityEngine;
using MyGame.Map;

/// <summary>
/// 플레이어를 특정 방의 중앙 위치로 이동시키는 역할을 전담하는 클래스
/// </summary>
public class PlayerPositioner : MonoBehaviour
{
    public Transform player;  // 이동시킬 플레이어 Transform 참조

    /// <summary>
    /// 전달받은 방(RoomData)의 중앙 위치로 플레이어 위치를 이동
    /// </summary>
    /// <param name="room">이동할 대상 방 데이터</param>
    public void MoveToRoomCenter(RoomData room)
    {
        // 방 타일의 중앙 월드 좌표를 가져와 플레이어 위치를 설정
        player.position = room.GetTileCenterWorldPos();
    }
}
