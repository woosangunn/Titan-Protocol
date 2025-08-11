using UnityEngine;
using MyGame.Map;

public class DoorTrigger : MonoBehaviour
{
    public Vector2Int direction; 
    public RoomLoader roomLoader; 

    private bool isOpen = false;

    public void Open()
    {
        isOpen = true;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DoorTrigger OnTriggerEnter2D by {other.name}");

        if (!isOpen) return;

        if (!other.CompareTag("Player")) return;

        roomLoader.TryMove(direction);
    }
}
