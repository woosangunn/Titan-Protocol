using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Vector2Int direction;  // 이 문이 연결된 방향(상,하,좌,우 중 하나)
    private bool isOpen = false;

    // 문 열림 상태로 변경, 활성화
    public void Open()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DoorTrigger 충돌 감지: isOpen={isOpen}, 충돌 대상={other.name}");

        if (!isOpen || !other.CompareTag("Player")) return;

        RoomLoader loader = FindAnyObjectByType<RoomLoader>();
        loader.TryMove(direction);
    }
}
