using UnityEngine;

public interface IRoomSpawner
{
    GameObject SpawnRoom(RoomData data, Vector2 worldPos);
    void DespawnRoom(RoomData data);
}
