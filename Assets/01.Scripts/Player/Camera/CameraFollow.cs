using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector2 offset;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + (Vector3)offset;
        transform.position = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, desiredPosition.y, -10f), smoothSpeed * Time.deltaTime);
    }
}
