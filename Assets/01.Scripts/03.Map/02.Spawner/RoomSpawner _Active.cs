using UnityEditor.EditorTools;
using UnityEngine;

public class RoomSpawner_Active : IRoomSpawner
{
    private RoomManager rm;

    public RoomSpawner_Active(RoomManager rm)
    {
        this.rm = rm;
    }

    public GameObject SpawnRoom(RoomData data, Vector2 worldPos)
    {
        if (data.instance == null)
        {
            data.instance = Object.Instantiate(rm.GetPrefab(data.type), worldPos, Quaternion.identity);
        }

        data.instance.SetActive(true);
        return data.instance;
    }

    public void DespawnRoom(RoomData data)
    {
        if (data.instance != null)
            data.instance.SetActive(false);
    }
}
