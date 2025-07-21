using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public GameObject startRoomPrefab;
    public GameObject combatRoomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject shopRoomPrefab;
    public GameObject bossRoomPrefab;

    public int roomCount = 10;
    public Vector2 roomSize = new Vector2(20, 12); // 각 방 사이 거리

    private Dictionary<Vector2Int, RoomData> roomMap;
    private Vector2Int currentPos;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var generator = new MapGenerator();
        roomMap = generator.GenerateMap(roomCount);
        currentPos = Vector2Int.zero;
        LoadRoom(currentPos);
    }

    public void TryMove(Vector2Int dir)
    {
        Vector2Int targetPos = currentPos + dir;
        if (!roomMap.ContainsKey(targetPos))
        {
            Debug.Log("이동할 방이 없음");
            return;
        }

        LoadRoom(targetPos);
    }

    void LoadRoom(Vector2Int pos)
    {
        // 이전 방 비활성화
        if (roomMap.ContainsKey(currentPos) && roomMap[currentPos].instance != null)
            roomMap[currentPos].instance.SetActive(false);

        RoomData room = roomMap[pos];

        // 아직 프리팹이 없다면 생성
        if (room.instance == null)
        {
            room.instance = Instantiate(GetPrefab(room.type), (Vector2)pos * roomSize, Quaternion.identity);
        }

        room.instance.SetActive(true);
        room.state = RoomState.Discovered;
        currentPos = pos;
    }

    GameObject GetPrefab(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startRoomPrefab,
            RoomType.Combat => combatRoomPrefab,
            RoomType.Treasure => treasureRoomPrefab,
            RoomType.Shop => shopRoomPrefab,
            RoomType.Boss => bossRoomPrefab,
            _ => combatRoomPrefab,
        };
    }

    public Vector2Int GetCurrentPosition() => currentPos;
}
