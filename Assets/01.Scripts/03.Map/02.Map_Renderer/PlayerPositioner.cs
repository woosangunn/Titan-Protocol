using UnityEngine;
using MyGame.Map;

public class PlayerPositioner : MonoBehaviour
{
    public Transform player; 

    public void MoveToRoomCenter(RoomData room)
    {
        player.position = room.GetTileCenterWorldPos();
    }
}
