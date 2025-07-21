using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    public Vector2Int direction;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomManager.Instance.TryMove(direction);
            other.transform.position = transform.position + (Vector3)((Vector2)direction * 2f);
        }
    }
}
