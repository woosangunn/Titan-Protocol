using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Vector2Int direction;
    public RoomLoader roomLoader;
    private bool isOpen = false;

    public void Open()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen || !other.CompareTag("Player")) return;
        roomLoader.TryMove(direction);
    }
}