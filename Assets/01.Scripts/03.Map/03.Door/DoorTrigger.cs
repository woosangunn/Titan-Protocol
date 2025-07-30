using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Vector2Int direction;  // �� ���� ����� ����(��,��,��,�� �� �ϳ�)
    private bool isOpen = false;

    // �� ���� ���·� ����, Ȱ��ȭ
    public void Open()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DoorTrigger �浹 ����: isOpen={isOpen}, �浹 ���={other.name}");

        if (!isOpen || !other.CompareTag("Player")) return;

        RoomLoader loader = FindAnyObjectByType<RoomLoader>();
        loader.TryMove(direction);
    }
}
