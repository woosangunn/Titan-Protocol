using UnityEngine;
using MyGame.Map;

public class RoomRenderer : MonoBehaviour
{
    public RoomTileRenderer tileRenderer; 
    public DoorRenderer doorRenderer; 

    public void Render(RoomData room, Vector3Int origin, RoomLoader loader)
    {
        tileRenderer.DrawTiles(room, origin);

        doorRenderer.SpawnDoors(room, origin, loader);
    }

    public void OpenAllDoors()
    {
        doorRenderer.OpenAllDoors();
    }
}
