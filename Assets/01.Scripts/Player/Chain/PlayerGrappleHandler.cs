// PlayerGrappleHandler.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGrappleHandler : MonoBehaviour
{
    public float grappleSpeed = 25f;
    public float swingForce = 10f;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer line;

    public bool IsAttached { get; private set; } = false;
    private Vector2 grapplePoint;

    private PlayerInput input;

    public bool IsGrappleActive()
    {
        return IsAttached;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();

        joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;
        joint.maxDistanceOnly = true;

        // �� �ʱ�ȭ
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

            // �罽 ���� ����: ��Ŭ�� �ٽ� ����
            if (input.RightClick)
            {
                Debug.Log("��Ŭ������ �罽 ���� �õ�");
                DetachGrapple();
            }
        }
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

        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, grapplePoint);
    }

    public void DoGrappleDash()
    {
        if (!IsAttached) return;

        DetachGrapple(); // ���� �� ����

        Vector2 dir = (grapplePoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * grappleSpeed;
    }

    public void DoSwing(float direction)
    {
        if (!IsAttached) return;

        DetachGrapple(); // ���� �� ����

        Vector2 force = new Vector2(0, 1f) + Vector2.right * direction;
        rb.AddForce(force.normalized * swingForce, ForceMode2D.Impulse);
    }

    public void DetachGrapple()
    {
        Debug.Log("�� �罽 ������");

        IsAttached = false;
        joint.enabled = false;
        line.enabled = false;
    }
}
