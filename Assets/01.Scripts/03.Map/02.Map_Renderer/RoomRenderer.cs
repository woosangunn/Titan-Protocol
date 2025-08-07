using UnityEngine;
using MyGame.Map;

/// <summary>
/// �� ������ ��Ʈ�ѷ�: Ÿ�� �������� �� �������� ���������� �����ϴ� Ŭ����
/// </summary>
public class RoomRenderer : MonoBehaviour
{
    public RoomTileRenderer tileRenderer;   // �ٴ� �� �� Ÿ���� �׸��� ������Ʈ
    public DoorRenderer doorRenderer;       // �� Ÿ�� �� �������� �������ϴ� ������Ʈ

    /// <summary>
    /// �� ��ü�� ������ (�ٴ�, ��, �� ����)
    /// </summary>
    /// <param name="room">�������� �� ������</param>
    /// <param name="origin">Ÿ�ϸ� �󿡼� ���� ���۵Ǵ� ���� ��ġ</param>
    /// <param name="loader">�� �̵� �� ����� RoomLoader ����</param>
    public void Render(RoomData room, Vector3Int origin, RoomLoader loader)
    {
        // �� Ÿ�� �׸��� (�ٴ�, ��)
        tileRenderer.DrawTiles(room, origin);

        // �� Ÿ�� + �� ������ ����
        doorRenderer.SpawnDoors(room, origin, loader);
    }

    /// <summary>
    /// ���� �濡 �ִ� ��� ���� ���� ��û ����
    /// </summary>
    public void OpenAllDoors()
    {
        doorRenderer.OpenAllDoors();
    }
}
