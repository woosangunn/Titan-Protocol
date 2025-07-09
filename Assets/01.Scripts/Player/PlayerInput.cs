using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 LastMove { get; private set; }
    public bool DashPressed { get; private set; }
    public bool LeftClick { get; private set; }
    public bool RightClick { get; private set; }
    public Vector2 MouseWorldPos2D => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    public void HandleInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;

        Vector2 input = new Vector2(h, v);
        Move = input.normalized;

        if (input != Vector2.zero)
            LastMove = Move;

        DashPressed = Input.GetKeyDown(KeyCode.Space);
        LeftClick = Input.GetMouseButtonDown(0);
        RightClick = Input.GetMouseButtonDown(1);
    }
}
