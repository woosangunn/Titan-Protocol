using UnityEditor.EditorTools;
using UnityEngine;

public class RoomSpawner_Instantiate : IRoomSpawner
{
    private RoomManager rm;

    public RoomSpawner_Instantiate(RoomManager rm)
    {
        this.rm = rm;
    }

    public GameObject SpawnRoom(RoomData data, Vector2 worldPos)
    {
        data.instance = Object.Instantiate(rm.GetPrefab(data.type), worldPos, Quaternion.identity);
        return data.instance;
    }

    public void DespawnRoom(RoomData data)
    {
        if (data.instance != null)
        {
            Object.Destroy(data.instance);
            data.instance = null;
        }
    }
}
