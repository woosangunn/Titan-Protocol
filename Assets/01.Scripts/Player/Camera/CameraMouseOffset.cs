using UnityEngine;

public class CameraMouseOffset : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public float maxOffset = 5f;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = target.position;

        Vector2 offsetDir = mouseWorldPos - targetPos;
        if (offsetDir.magnitude > maxOffset)
            offsetDir = offsetDir.normalized * maxOffset;

        Vector3 desiredPos = targetPos + (Vector3)offsetDir;
        transform.position = Vector3.Lerp(transform.position, new Vector3(desiredPos.x, desiredPos.y, -10f), followSpeed * Time.deltaTime);
    }
}
