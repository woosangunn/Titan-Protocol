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
    public Vector2 roomSize = new Vector2(20, 12); // �� �� ���� �Ÿ�

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
            Debug.Log("�̵��� ���� ����");
            return;
        }

        LoadRoom(targetPos);
    }

    void LoadRoom(Vector2Int pos)
    {
        // ���� �� ��Ȱ��ȭ
        if (roomMap.ContainsKey(currentPos) && roomMap[currentPos].instance != null)
            roomMap[currentPos].instance.SetActive(false);

        RoomData room = roomMap[pos];

        // ���� �������� ���ٸ� ����
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
