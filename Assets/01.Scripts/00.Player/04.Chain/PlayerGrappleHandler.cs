using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGrappleHandler : MonoBehaviour
{
    [Header("Chain Gauge")]
    public int dashCost = 10;
    public int swingCost = 1;
    public float swingTickInterval = 0.2f;
    public float rechargeInterval = 0.06f;
    public int CurrentGauge { get; private set; } = 30;
    public int maxGauge = 30;
    public event Action<int, int> OnGaugeChanged;

    [Header("Grapple Movement")]
    public float grappleSpeed = 20f;
    public float swingForce = 10f;
    public float detachDistance = 0.3f;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer line;

    public bool IsAttached { get; private set; } = false;

    private Vector2 grapplePoint;

    private PlayerInput input;
    private Coroutine dashRoutine;
    private Coroutine rechargeRoutine;

    private bool isSwinging = false;
    private float swingDir = 0f;
    private float swingTickTimer = 0f;

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
        line.startWidth = line.endWidth = 0.05f;
        line.enabled = false;

        CurrentGauge = maxGauge;
        OnGaugeChanged?.Invoke(CurrentGauge, maxGauge);
    }

    void Update()
    {
        if (!IsAttached) return;

        if (isSwinging)
        {
            swingTickTimer += Time.deltaTime;
            if (swingTickTimer >= swingTickInterval)
            {
                swingTickTimer = 0f;
                if (!ConsumeGauge(swingCost))
                {
                    DetachGrapple();
                    return;
                }
            }
        }

        if (input.RightClick)
            DetachGrapple();

        line.SetPosition(0, transform.position);
        line.SetPosition(1, grapplePoint);
    }

    void FixedUpdate()
    {
        if (isSwinging && IsAttached)
        {
            Vector2 toAnchor = grapplePoint - (Vector2)transform.position;
            Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * -Mathf.Sign(swingDir);
            rb.AddForce(tangent * swingForce, ForceMode2D.Force);
        }
    }

    public void AttachGrapple(Vector2 point)
    {
        if (IsAttached) return;
        if (CurrentGauge <= 0) return;

        if (rechargeRoutine != null)
        {
            StopCoroutine(rechargeRoutine);
            rechargeRoutine = null;
        }

        IsAttached = true;
        grapplePoint = point;

        joint.enabled = true;
        joint.connectedAnchor = grapplePoint;
        joint.distance = Vector2.Distance(transform.position, grapplePoint);
        joint.enableCollision = true;

        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, grapplePoint);

        rb.linearVelocity = Vector2.zero;
        swingTickTimer = 0f;
    }

    public void DoSwing(float direction)
    {
        if (!IsAttached) return;

        swingDir = direction;
        isSwinging = true;

        Vector2 toAnchor = grapplePoint - (Vector2)transform.position;
        Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * -Mathf.Sign(swingDir);
        rb.AddForce(tangent * swingForce, ForceMode2D.Impulse);
    }

    public void DoGrappleDash()
    {
        if (!IsAttached) return;

        if (!ConsumeGauge(dashCost))
        {
            DetachGrapple();
            return;
        }

        if (dashRoutine != null) StopCoroutine(dashRoutine);
        dashRoutine = StartCoroutine(GrappleDashCoroutine());
    }

    public void DetachGrapple()
    {
        if (!IsAttached) return;

        IsAttached = false;
        isSwinging = false;
        swingDir = 0f;

        joint.enabled = false;
        line.enabled = false;

        if (rechargeRoutine != null) StopCoroutine(rechargeRoutine);
        rechargeRoutine = StartCoroutine(RechargeCoroutine());
    }

    IEnumerator GrappleDashCoroutine()
    {
        while (IsAttached)
        {
            Vector2 toTarget = grapplePoint - (Vector2)transform.position;

            if (toTarget.magnitude < detachDistance)
            {
                rb.linearVelocity = Vector2.zero;
                DetachGrapple();
                dashRoutine = null;
                yield break;
            }

            rb.linearVelocity = toTarget.normalized * grappleSpeed;
            yield return new WaitForFixedUpdate();
        }
        dashRoutine = null;
    }

    IEnumerator RechargeCoroutine()
    {
        while (CurrentGauge < maxGauge)
        {
            yield return new WaitForSeconds(rechargeInterval);
            CurrentGauge++;
            OnGaugeChanged?.Invoke(CurrentGauge, maxGauge);
        }
        rechargeRoutine = null;
    }

    private bool ConsumeGauge(int amount)
    {
        if (CurrentGauge < amount) return false;
        CurrentGauge -= amount;
        OnGaugeChanged?.Invoke(CurrentGauge, maxGauge);
        return true;
    }
}
