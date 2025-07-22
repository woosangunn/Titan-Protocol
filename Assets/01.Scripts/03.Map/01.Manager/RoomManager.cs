using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Room Prefabs")]
    public GameObject startRoomPrefab;
    public GameObject combatRoomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject shopRoomPrefab;
    public GameObject bossRoomPrefab;

    [Header("Settings")]
    public int roomCount = 10;
    public Vector2 roomSize = new Vector2(20, 12);

    private Dictionary<Vector2Int, RoomData> roomMap;
    private Vector2Int currentPos;

    private IRoomSpawner spawner;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        roomMap = new MapGenerator().GenerateMap(roomCount);
        currentPos = Vector2Int.zero;

        // 원하는 전략으로 선택
        spawner = new RoomSpawner_Active(this);       // 또는 new RoomSpawner_Instantiate(this)

        // 사전 생성 방식일 경우 전체 생성
        if (spawner is RoomSpawner_Active)
        {
            foreach (var kv in roomMap)
            {
                Vector2 pos = kv.Key * roomSize;
                spawner.SpawnRoom(kv.Value, pos);
                spawner.DespawnRoom(kv.Value);
            }
        }

        LoadRoom(currentPos);
    }

    public void TryMove(Vector2Int dir)
    {
        Vector2Int nextPos = currentPos + dir;
        if (!roomMap.ContainsKey(nextPos))
        {
            Debug.Log("이동할 방이 없음");
            return;
        }

        LoadRoom(nextPos);
    }

    void LoadRoom(Vector2Int pos)
    {
        spawner.DespawnRoom(roomMap[currentPos]);

        RoomData targetRoom = roomMap[pos];
        Vector2 worldPos = (Vector2)pos * roomSize;
        spawner.SpawnRoom(targetRoom, worldPos);

        targetRoom.state = RoomState.Discovered;
        currentPos = pos;
    }

    public GameObject GetPrefab(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startRoomPrefab,
            RoomType.Combat => combatRoomPrefab,
            RoomType.Treasure => treasureRoomPrefab,
            RoomType.Shop => shopRoomPrefab,
            RoomType.Boss => bossRoomPrefab,
            _ => combatRoomPrefab
        };
    }

    public Vector2Int GetCurrentPosition() => currentPos;
}
