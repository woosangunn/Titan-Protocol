using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGrappleHandler : MonoBehaviour
{
    public float grappleSpeed = 100f;
    public float swingForce = 50f;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer line;

    public bool IsAttached { get; private set; } = false;
    private Vector2 grapplePoint;

    private PlayerInput input;

    private float swingDirection = 0f;
    private bool isSwinging = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();

        joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;
        joint.maxDistanceOnly = true;

        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.enabled = false;
    }

    void Update()
    {
        if (IsAttached)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, grapplePoint);

            if (input.RightClick)
                DetachGrapple();
        }
    }

    void FixedUpdate()
    {
        if (isSwinging && IsAttached)
        {
            Vector2 toAnchor = grapplePoint - (Vector2)transform.position;
            Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * Mathf.Sign(swingDirection);

            rb.AddForce(tangent * swingForce, ForceMode2D.Impulse);
            Debug.Log($"⒑⒑ SWING 塞 利侩: {tangent * swingForce}");
        }
    }

    public void AttachGrapple(Vector2 point)
    {
        if (IsAttached) return;

        IsAttached = true;
        grapplePoint = point;

        joint.enabled = true;
        joint.connectedAnchor = grapplePoint;
        joint.distance = Vector2.Distance(transform.position, grapplePoint);
        joint.enableCollision = true;

        rb.linearVelocity = Vector2.zero;

        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, grapplePoint);
    }

    public void DoGrappleDash()
    {
        if (!IsAttached) return;

        Vector2 dir = (grapplePoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * grappleSpeed;

        Debug.Log($"⒑⒑ GRAPPLE DASH 角青凳 | 规氢: {dir}, 加档: {grappleSpeed}");

        Invoke(nameof(DetachGrapple), 0.2f);
    }

    public void DoSwing(float direction)
    {
        if (!IsAttached) return;

        swingDirection = direction;
        isSwinging = true;
    }

    public void DetachGrapple()
    {
        if (!IsAttached) return;

        IsAttached = false;
        isSwinging = false;
        swingDirection = 0f;

        joint.enabled = false;
        line.enabled = false;
    }
}
