using System.Collections.Generic;
using UnityEngine;
using MyGame.Map;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public int roomCount = 10;                            // ������ �� ����
    public Vector2Int tileOffsetStep = new Vector2Int(50, 40);  // �� �� ���� (Ÿ�� ����)

    private Dictionary<Vector2Int, RoomData> roomMap;    // ��ġ�� �� ������ ��
    private Vector2Int currentPos;                        // ���� �÷��̾ ��ġ�� �� ��ǥ

    private RoomTileRenderer tileRenderer;

    private Vector3Int lastRoomOffset;
    private Vector2 lastRoomSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        tileRenderer = GetComponent<RoomTileRenderer>();
    }

    private void Start()
    {
        // 1. �� ����
        roomMap = new MapGenerator().GenerateMap(roomCount);

        // 2. ���� ��ġ�� Start �� ��ġ (0,0) ����
        currentPos = Vector2Int.zero;

        // 3. ��� �� Ÿ�ϸʿ� �׸��� (���� ���� �游)
        DrawAllRooms();

        // 4. �÷��̾� ���� ��ġ ���� (�� �߽� ����)
        PositionPlayerToCurrentRoomCenter();

        // 5. ���� �� ���� ����
        SetRoomDiscovered(currentPos);
    }

    /// <summary>
    /// ��� ���� Ÿ�ϸʿ� �׸��� (���� ����)
    /// </summary>
    private void DrawAllRooms()
    {
        if (tileRenderer == null) return;

        foreach (var kvp in roomMap)
        {
            RoomData room = kvp.Value;
            Vector3Int offset = new Vector3Int(room.position.x * tileOffsetStep.x, room.position.y * tileOffsetStep.y, 0);

            tileRenderer.DrawRoom(room, offset);
        }
    }

    /// <summary>
    /// �÷��̾ ���� �� �߽����� ��ġ�ϰ� ��
    /// </summary>
    private void PositionPlayerToCurrentRoomCenter()
    {
        RoomData currentRoom = GetRoomData(currentPos);
        if (currentRoom == null)
        {
            Debug.LogWarning("���� �� �����Ͱ� �����ϴ�.");
            return;
        }

        // �� Ÿ�� ��ǥ �������� �÷��̾� ��ġ ��� (�߾�)
        Vector3Int offset = new Vector3Int(currentPos.x * tileOffsetStep.x, currentPos.y * tileOffsetStep.y, 0);
        Vector2 roomCenter = new Vector2(offset.x + currentRoom.size.x / 2f, offset.y + currentRoom.size.y / 2f);

        // �÷��̾� ������Ʈ�� ã�� ��ġ �̵� (�±� "Player"�� ����)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(roomCenter.x, roomCenter.y, player.transform.position.z);
        }
    }

    /// <summary>
    /// Ư�� �������� �� �̵� �õ�
    /// </summary>
    /// <param name="dir">�̵� ���� (Vector2Int.up, down, left, right)</param>
    public void TryMove(Vector2Int dir)
    {
        Vector2Int nextPos = currentPos + dir;
        if (!roomMap.ContainsKey(nextPos))
        {
            Debug.Log("�̵��� ���� �����ϴ�.");
            return;
        }

        LoadRoom(nextPos);
    }

    /// <summary>
    /// ���� �ε��ϰ� �÷��̾� ��ġ ����
    /// </summary>
    /// <param name="pos">�ε��� �� ��ǥ</param>
    private void LoadRoom(Vector2Int pos)
    {
        RoomData room = GetRoomData(pos);
        if (room == null)
        {
            Debug.LogWarning("�������� �ʴ� �� ��ġ�Դϴ�.");
            return;
        }

        // ���� �� Ŭ����� ���⼱ ���� (��ü �� �׸��Ƿ�)

        currentPos = pos;

        // �÷��̾� ��ġ �̵�
        PositionPlayerToCurrentRoomCenter();

        // �� �湮 ���� ����
        SetRoomDiscovered(pos);

        Debug.Log($"���� �� ��ġ: {pos}, Ÿ��: {room.type}");
    }

    /// <summary>
    /// �� �湮 ���¸� Discovered�� ����
    /// </summary>
    private void SetRoomDiscovered(Vector2Int pos)
    {
        RoomData room = GetRoomData(pos);
        if (room != null && room.state == RoomState.Unvisited)
        {
            room.state = RoomState.Discovered;
        }
    }

    /// <summary>
    /// Ư�� �� ������ ��ȯ (������ null)
    /// </summary>
    public RoomData GetRoomData(Vector2Int pos)
    {
        if (roomMap != null && roomMap.TryGetValue(pos, out RoomData room))
            return room;
        return null;
    }

    /// <summary>
    /// ���� �÷��̾ ��ġ�� �� ��ǥ ��ȯ
    /// </summary>
    public Vector2Int GetCurrentPosition()
    {
        return currentPos;
    }
}
