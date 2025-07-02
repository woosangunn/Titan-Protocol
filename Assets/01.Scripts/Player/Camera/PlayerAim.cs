using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Transform[] aimPivots; // 총 회전용 오브젝트
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        foreach (Transform pivot in aimPivots)
        {
            if (pivot == null) continue;

            Vector2 dir = (mouseWorldPos - (Vector2)pivot.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            pivot.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
