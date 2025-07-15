using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGrappleHandler : MonoBehaviour
{
    [Header("그래플링 세팅")]
    public float grappleSpeed = 20f;             // 사슬 대시 시 이동 속도
    public float swingForce = 10f;               // 스윙 중 지속적으로 가해지는 힘
    public float detachDelay = 0.2f;             // (미사용) 대시 후 분리 지연 시간

    [Header("사슬 시간 설정")]
    public float maxAttachTime = 3f;             // 체인 최대 지속 시간

    [Header("스윙 게이지 설정")]
    public float swingEnergyPerSecond = 0.1f;    // 스윙 시 초당 소모량
    public float swingRechargePerSecond = 0.05f; // 스윙 시 아닐 때 초당 회복량
    public float swingEnergyTickInterval = 0.001f;    // 소모 적용 틱 간격
    public float initialSwingBoost = 5f;             // 스윙 시작 시 단발 초기 힘

    [Header("대시 게이지 설정")]
    public float dashEnergyCost = 1.0f;          // 대시 시 1회 소모량
    public float dashRechargePerSecond = 0.1f;   // 대시 게이지 초당 회복량

    private float swingEnergyPerTick;
    private float swingRechargePerTick;
    private float swingEnergyTickTimer = 0f;

    private float attachTimer = 0f;               // 스윙 누적 소모
    private float dashUsed = 0f;                   // 대시 누적 소모

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer line;

    public bool IsAttached { get; private set; }
    private Vector2 grapplePoint;

    private PlayerInput input;
    private GhostSpawner ghostSpawner;

    private float swingDirection;
    private bool isSwinging;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        ghostSpawner = GetComponent<GhostSpawner>();

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

        swingEnergyPerTick = swingEnergyPerSecond * swingEnergyTickInterval;
        swingRechargePerTick = swingRechargePerSecond * swingEnergyTickInterval;
    }

    void Update()
    {
        if (IsAttached)
        {
            if (isSwinging)
            {
                // 스윙 중 체인 소모 처리 (틱 단위)
                swingEnergyTickTimer += Time.deltaTime;

                while (swingEnergyTickTimer >= swingEnergyTickInterval)
                {
                    swingEnergyTickTimer -= swingEnergyTickInterval;
                    attachTimer += swingEnergyPerTick;

                    if (attachTimer + dashUsed >= maxAttachTime)
                    {
                        DetachGrapple();
                        return;
                    }
                }
            }
            else
            {
                // 스윙 중이 아닐 때는 스윙 게이지 회복
                if (attachTimer > 0f)
                {
                    swingEnergyTickTimer += Time.deltaTime;

                    while (swingEnergyTickTimer >= swingEnergyTickInterval)
                    {
                        swingEnergyTickTimer -= swingEnergyTickInterval;
                        attachTimer -= swingRechargePerTick;
                        attachTimer = Mathf.Max(attachTimer, 0f);
                    }
                }
            }

            // 대시 입력 감지 및 소모 처리
            if (input.DashPressed)
                TryConsumeDashEnergy();

            // 대시 게이지도 회복 처리
            if (dashUsed > 0f)
            {
                dashUsed -= dashRechargePerSecond * Time.deltaTime;
                dashUsed = Mathf.Max(dashUsed, 0f);
            }

            // 라인 렌더러 위치 갱신
            line.SetPosition(0, transform.position);
            line.SetPosition(1, grapplePoint);

            // 오른쪽 클릭 시 사슬 분리
            if (input.RightClick)
                DetachGrapple();
        }
        else
        {
            // 사슬이 떨어졌을 때 스윙 & 대시 게이지 회복
            if (attachTimer > 0f)
            {
                attachTimer -= swingRechargePerSecond * Time.deltaTime;
                attachTimer = Mathf.Max(attachTimer, 0f);
            }

            if (dashUsed > 0f)
            {
                dashUsed -= dashRechargePerSecond * Time.deltaTime;
                dashUsed = Mathf.Max(dashUsed, 0f);
            }
        }
    }

    void FixedUpdate()
    {
        if (isSwinging && IsAttached)
        {
            Vector2 toAnchor = grapplePoint - (Vector2)transform.position;
            Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * -Mathf.Sign(swingDirection);
            rb.AddForce(tangent * swingForce, ForceMode2D.Force);
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

        ghostSpawner?.StartGhostTrail();

        attachTimer = 0f;
        dashUsed = 0f;
        swingEnergyTickTimer = 0f;
        isSwinging = false;
    }

    public void DoSwing(float direction)
    {
        if (!IsAttached) return;

        swingDirection = direction;
        isSwinging = true;

        // 초기 스윙 부스트 (단발 Impulse)
        Vector2 toAnchor = grapplePoint - (Vector2)transform.position;
        Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * -Mathf.Sign(swingDirection);
        rb.AddForce(tangent * initialSwingBoost, ForceMode2D.Impulse);

        ghostSpawner?.StartGhostTrail();
    }

    public void DoGrappleDash()
    {
        if (!IsAttached) return;

        StopAllCoroutines();
        StartCoroutine(GrappleDashCoroutine());

        TryConsumeDashEnergy();
    }

    private IEnumerator GrappleDashCoroutine()
    {
        while (true)
        {
            Vector2 toTarget = grapplePoint - (Vector2)transform.position;

            if (toTarget.magnitude < 0.3f)
            {
                rb.linearVelocity = Vector2.zero;
                DetachGrapple();
                yield break;
            }

            rb.linearVelocity = toTarget.normalized * grappleSpeed;
            yield return new WaitForFixedUpdate();
        }
    }

    private void TryConsumeDashEnergy()
    {
        if (dashUsed + dashEnergyCost > maxAttachTime)
        {
            // 대시 소모하면 총 체인 제한 초과 => 분리
            DetachGrapple();
            return;
        }

        dashUsed += dashEnergyCost;
    }

    public void DetachGrapple()
    {
        if (!IsAttached) return;

        IsAttached = false;
        isSwinging = false;
        swingDirection = 0f;

        joint.enabled = false;
        line.enabled = false;

        ghostSpawner?.StopGhostTrail();

        attachTimer = 0f;
        dashUsed = 0f;
        swingEnergyTickTimer = 0f;
    }

    // UI용 남은 게이지 (스윙 + 대시 합산)
    public float RemainingChainTime()
    {
        return Mathf.Clamp(maxAttachTime - (attachTimer + dashUsed), 0f, maxAttachTime);
    }

    // UI용 개별 게이지 상태 반환
    public float SwingChainRatio()
    {
        return Mathf.Clamp01(1f - attachTimer / maxAttachTime);
    }

    public float DashChainRatio()
    {
        return Mathf.Clamp01(1f - dashUsed / maxAttachTime);
    }
}
