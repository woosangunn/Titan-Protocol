using MyGame.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// �� ������ �� �� ������ ��ġ�� ����ϴ� ������Ʈ
/// �� Ÿ��, �� ������Ʈ, �� Ʈ���Ÿ� ���� �� ������
/// </summary>
public class DoorRenderer : MonoBehaviour
{
    public Tilemap wallTilemap;      // �� Ÿ���� �׸� �� Ÿ�ϸ�
    public TileBase doorTile;        // �� Ÿ�Ϸ� ����� Ÿ��
    public GameObject doorPrefab;    // �� ������ (�� Ʈ���� ����)

    private List<GameObject> spawnedDoors = new(); // ������ �� ������ ����Ʈ

    /// <summary>
    /// ���޹��� ��(RoomData)�� �� ������ ������� �� Ÿ�ϰ� �������� ����
    /// </summary>
    /// <param name="room">���� �� ������</param>
    /// <param name="origin">���� ���� ��ǥ (Ÿ�ϸ� ����)</param>
    /// <param name="loader">�� �̵��� ����ϴ� RoomLoader ����</param>
    public void SpawnDoors(RoomData room, Vector3Int origin, RoomLoader loader)
    {
        ClearDoors(); // ���� �� ����

        foreach (var doorData in room.doors)
        {
            // �� ���� + ���� �� ��ġ = Ÿ�ϸ� �� �� ��ġ
            Vector3Int tilePos = origin + (Vector3Int)doorData.localPosition;

            // Ÿ�Ϸ� �� �׸��� (���û���)
            if (doorTile != null)
                wallTilemap.SetTile(tilePos, doorTile);

            // �� ������ ���� (Ÿ�ϸ� ���� ���� ��ǥ�� ��ȯ�ؼ� ��ġ ����)
            Vector3 worldPos = wallTilemap.CellToWorld(tilePos);
            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity);

            // DoorTrigger ���� (�� �̵� ���� �� �δ� ����)
            if (door.TryGetComponent(out DoorTrigger trigger))
            {
                trigger.direction = doorData.direction;   // ���� ���� ���� (��: �������̸� �÷��̾�� �������� ������ ��)
                trigger.roomLoader = loader;

                // �������� �ƴ� ���, ó������ ���� �����
                if (room.type != RoomType.Combat)
                    trigger.Open();
            }

            // ������ ���� ����Ʈ�� ������ ���߿� �����ϰų� �� �� �ְ� ��
            spawnedDoors.Add(door);
        }
    }

    /// <summary>
    /// ������ ��� ���� ������ ��û
    /// </summary>
    public void OpenAllDoors()
    {
        foreach (var door in spawnedDoors)
        {
            if (door.TryGetComponent(out DoorTrigger trigger))
                trigger.Open();
        }
    }

    /// <summary>
    /// ������ ������ �� �����յ��� ���� ����
    /// </summary>
    public void ClearDoors()
    {
        foreach (var door in spawnedDoors)
            if (door) Destroy(door);

        spawnedDoors.Clear();
    }
}
