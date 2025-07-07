// PlayerGrappleHandler.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGrappleHandler : MonoBehaviour
{
    public float grappleSpeed = 25f;
    public float swingForce = 10f;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;

    public bool IsAttached { get; private set; } = false;
    private Vector2 grapplePoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;
        joint.maxDistanceOnly = true;
    }

    public void AttachGrapple(Vector2 point)
    {
        IsAttached = true;
        grapplePoint = point;

        joint.enabled = true;
        joint.connectedAnchor = grapplePoint;
        joint.distance = Vector2.Distance(transform.position, grapplePoint);
        joint.enableCollision = true;

        rb.linearVelocity = Vector2.zero;
    }

    public void DoGrappleDash()
    {
        if (!IsAttached) return;

        joint.enabled = false;
        IsAttached = false;

        Vector2 dir = (grapplePoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * grappleSpeed;
    }

    public void DoSwing(float direction)
    {
        if (!IsAttached) return;

        Vector2 force = new Vector2(0, 1f) + Vector2.right * direction;
        rb.AddForce(force.normalized * swingForce, ForceMode2D.Impulse);
    }

    public void DetachGrapple()
    {
        IsAttached = false;
        joint.enabled = false;
    }
}
