using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGrappleHandler : MonoBehaviour
{
    [Header("Chain Gauge")]
    [Tooltip("대시에 필요한 체인 게이지 소모량")]
    public int dashCost = 10;
    [Tooltip("스윙 시 일정 간격으로 소모되는 게이지")]
    public int swingCost = 1;
    [Tooltip("스윙 게이지 소모 주기 (초)")]
    public float swingTickInterval = 0.2f;
    [Tooltip("게이지 충전 주기 (초)")]
    public float rechargeInterval = 0.06f;
    [Tooltip("현재 체인 게이지")]
    public int CurrentGauge { get; private set; } = 30;
    [Tooltip("최대 체인 게이지")]
    public int maxGauge = 30;
    [Tooltip("체인 게이지 변경 이벤트 (현재, 최대)")]
    public event Action<int, int> OnGaugeChanged;

    [Header("Grapple Movement")]
    [Tooltip("그랩 대시 속도")]
    public float grappleSpeed = 20f;
    [Tooltip("스윙할 때 적용할 힘")]
    public float swingForce = 10f;
    [Tooltip("그랩 대상에 너무 가까워지면 자동 해제 거리")]
    public float detachDistance = 0.3f;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private LineRenderer line;

    [Tooltip("그랩 상태 플래그")]
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

        // DistanceJoint2D 컴포넌트 동적 생성 및 초기화
        joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;
        joint.maxDistanceOnly = true;

        // 라인렌더러 생성 및 기본 설정
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = line.endWidth = 0.05f;
        line.enabled = false;

        CurrentGauge = maxGauge;
        OnGaugeChanged?.Invoke(CurrentGauge, maxGauge); // 초기 게이지 UI 갱신
    }

    void Update()
    {
        if (!IsAttached) return;

        // 스윙 중일 때 일정 간격으로 게이지 소모 처리
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

        // 마우스 우클릭 시 그랩 해제
        if (input.RightClick)
            DetachGrapple();

        // 그랩 줄 렌더링 위치 갱신
        line.SetPosition(0, transform.position);
        line.SetPosition(1, grapplePoint);
    }

    void FixedUpdate()
    {
        if (isSwinging && IsAttached)
        {
            Vector2 toAnchor = grapplePoint - (Vector2)transform.position;
            // 접선 방향 계산 (스윙 방향에 따라 부호 반전)
            Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * -Mathf.Sign(swingDir);
            // 접선 방향으로 힘 가하기 (스윙 구현)
            rb.AddForce(tangent * swingForce, ForceMode2D.Force);
        }
    }

    /// <summary>
    /// 그랩 대상에 붙기
    /// </summary>
    public void AttachGrapple(Vector2 point)
    {
        if (IsAttached) return;
        if (CurrentGauge <= 0) return;

        // 충전 코루틴 중지
        if (rechargeRoutine != null)
        {
            StopCoroutine(rechargeRoutine);
            rechargeRoutine = null;
        }

        IsAttached = true;
        grapplePoint = point;

        // DistanceJoint 설정 및 활성화
        joint.enabled = true;
        joint.connectedAnchor = grapplePoint;
        joint.distance = Vector2.Distance(transform.position, grapplePoint);
        joint.enableCollision = true;

        // 라인렌더러 활성화 및 위치 초기화
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, grapplePoint);

        rb.linearVelocity = Vector2.zero;  // 현재 속도 초기화
        swingTickTimer = 0f;
    }

    /// <summary>
    /// 그랩 줄을 이용한 스윙 시작
    /// </summary>
    public void DoSwing(float direction)
    {
        if (!IsAttached) return;

        swingDir = direction;
        isSwinging = true;

        // 처음 스윙 시작할 때 접선 방향으로 임펄스 힘 가하기
        Vector2 toAnchor = grapplePoint - (Vector2)transform.position;
        Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * -Mathf.Sign(swingDir);
        rb.AddForce(tangent * swingForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 그랩 줄을 이용한 대시 시작
    /// </summary>
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

    /// <summary>
    /// 그랩 줄 해제
    /// </summary>
    public void DetachGrapple()
    {
        if (!IsAttached) return;

        IsAttached = false;
        isSwinging = false;
        swingDir = 0f;

        joint.enabled = false;
        line.enabled = false;

        // 게이지 충전 코루틴 시작
        if (rechargeRoutine != null) StopCoroutine(rechargeRoutine);
        rechargeRoutine = StartCoroutine(RechargeCoroutine());
    }

    /// <summary>
    /// 그랩 대시 코루틴: 목표점까지 일정 속도로 이동
    /// </summary>
    IEnumerator GrappleDashCoroutine()
    {
        while (IsAttached)
        {
            Vector2 toTarget = grapplePoint - (Vector2)transform.position;

            // 목표점에 너무 가까워지면 대시 종료 및 정지
            if (toTarget.magnitude < detachDistance)
            {
                rb.linearVelocity = Vector2.zero;
                DetachGrapple();
                yield break;
            }

            rb.linearVelocity = toTarget.normalized * grappleSpeed;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 체인 게이지를 일정 간격으로 충전하는 코루틴
    /// </summary>
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

    /// <summary>
    /// 게이지 소모 함수 (소모 가능하면 true 반환)
    /// </summary>
    private bool ConsumeGauge(int amount)
    {
        if (CurrentGauge < amount) return false;
        CurrentGauge -= amount;
        OnGaugeChanged?.Invoke(CurrentGauge, maxGauge);
        return true;
    }
}
