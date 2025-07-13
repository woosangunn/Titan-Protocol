using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGrappleHandler : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grappleSpeed = 20f;     // 사슬 대시 시 이동 속도
    public float swingForce = 10f;       // 스윙할 때 가해지는 힘 크기
    public float detachDelay = 0.2f;     // 대시 후 사슬이 분리되기까지의 지연 시간

    private Rigidbody2D rb;              // 플레이어 Rigidbody2D 컴포넌트
    private DistanceJoint2D joint;       // 플레이어와 사슬 포인트를 연결하는 DistanceJoint2D
    private LineRenderer line;           // 사슬을 시각적으로 표현하는 라인렌더러
    // private TrailRenderer trail;      // 이동 궤적을 그리는 TrailRenderer (현재 주석 처리됨)

    public bool IsAttached { get; private set; }    // 사슬이 현재 벽/오브젝트에 연결되어 있는지 상태
    private Vector2 grapplePoint;                     // 사슬이 연결된 지점 좌표

    private PlayerInput input;            // 플레이어 입력 정보 컴포넌트

    // 스윙 관련 변수들
    private float swingDirection;         // 스윙 방향 (-1: 왼쪽, 1: 오른쪽)
    private bool isSwinging;              // 현재 스윙 상태인지 여부

    private GhostSpawner ghostSpawner;    // 이동 궤적(고스트 트레일) 생성기 컴포넌트

    public float maxAttachTime = 3f;
    private float attachTimer = 0f;

    public float RemainingChainTime()
    {
        return Mathf.Clamp(maxAttachTime - attachTimer, 0f, maxAttachTime);
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();            // Rigidbody2D 컴포넌트 가져오기
        input = GetComponent<PlayerInput>();          // PlayerInput 컴포넌트 가져오기
        ghostSpawner = GetComponent<GhostSpawner>();  // GhostSpawner 컴포넌트 가져오기

        joint = gameObject.AddComponent<DistanceJoint2D>();  // DistanceJoint2D 컴포넌트 추가 및 설정
        joint.enabled = false;                        // 초기엔 비활성화
        joint.autoConfigureDistance = false;          // 거리 직접 설정
        joint.maxDistanceOnly = true;                  // 최대 거리만 제한 (거리 고정)

        line = gameObject.AddComponent<LineRenderer>();      // LineRenderer 컴포넌트 추가 및 설정
        line.positionCount = 2;                         // 두 점으로 라인 설정 (플레이어 위치와 연결 지점)
        line.material = new Material(Shader.Find("Sprites/Default"));  // 기본 스프라이트 셰이더 사용
        line.startWidth = 0.05f;                        // 라인 시작 두께
        line.endWidth = 0.05f;                          // 라인 끝 두께
        line.enabled = false;                           // 초기엔 비활성화

        // trail = GetComponent<TrailRenderer>();
        // if (trail != null) trail.enabled = false;    // TrailRenderer 비활성화 (현재 주석 처리됨)
    }

    void Update()
    {
        if (IsAttached)
            attachTimer += Time.deltaTime;
        else
            attachTimer = 0f;

        if (IsAttached)
        {
            // 사슬 라인 시작점은 플레이어 위치, 끝점은 grapplePoint (연결된 지점)
            line.SetPosition(0, transform.position);
            line.SetPosition(1, grapplePoint);

            // 오른쪽 클릭 입력 시 사슬 분리
            if (input.RightClick)
                DetachGrapple();
        }
    }

    void FixedUpdate()
    {
        // 스윙 중일 때 사슬 연결 상태라면 스윙 힘 적용
        if (isSwinging && IsAttached)
        {
            Vector2 toAnchor = grapplePoint - (Vector2)transform.position;            // 플레이어에서 연결점 방향 벡터
            Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized) * -Mathf.Sign(swingDirection);  // 접선 방향 벡터 (스윙 방향에 따라 부호 조정)
            rb.AddForce(tangent * swingForce, ForceMode2D.Force);                     // 접선 방향으로 힘 추가해 스윙 동작 구현
        }
    }

    // 사슬 연결 함수
    public void AttachGrapple(Vector2 point)
    {
        if (IsAttached) return;    // 이미 연결되어 있으면 무시

        IsAttached = true;         // 연결 상태로 변경
        grapplePoint = point;      // 연결 지점 설정

        joint.enabled = true;                  // DistanceJoint 활성화
        joint.connectedAnchor = grapplePoint; // 연결 앵커 위치 설정
        joint.distance = Vector2.Distance(transform.position, grapplePoint);   // 현재 플레이어 위치와 연결점 사이 거리로 조절
        joint.enableCollision = true;         // 플레이어와 연결 지점 사이 충돌 활성화

        rb.linearVelocity = Vector2.zero;     // 현재 속도 초기화

        line.enabled = true;                   // 라인렌더러 활성화해 사슬 표시
        line.SetPosition(0, transform.position);
        line.SetPosition(1, grapplePoint);

        // EnableTrail(true);                  // 이동 궤적 활성화 (현재 주석 처리됨)
        ghostSpawner?.StartGhostTrail();       // 고스트 트레일 시작 (null 체크 포함 호출)
    }

    // 사슬을 사용해 대시 이동 수행
    public void DoGrappleDash()
    {
        if (!IsAttached) return;    // 연결 안되어 있으면 무시

        // 사슬 지점까지 직선 돌진 (프레임 단위 처리)
        StopAllCoroutines();
        StartCoroutine(GrappleDashCoroutine());

        //Vector2 dir = (grapplePoint - (Vector2)transform.position).normalized;  // 연결 지점 방향 단위벡터
        //rb.linearVelocity = dir * grappleSpeed;        // 해당 방향으로 속도 세팅 (대시 이동)

        //Invoke(nameof(StopAndDetach), detachDelay);    // 일정 시간 후 대시 멈추고 사슬 분리 호출
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

    // 대시 멈추고 사슬 분리 함수
    void StopAndDetach()
    {
        DetachGrapple();
    }

    // 스윙 시작 함수 (방향값 -1 또는 1 전달)
    public void DoSwing(float direction)
    {
        if (!IsAttached) return;    // 연결 안되어 있으면 무시

        swingDirection = direction; // 스윙 방향 설정
        isSwinging = true;          // 스윙 상태 활성화
        // EnableTrail(true);       // 이동 궤적 활성화 (현재 주석 처리됨)
        ghostSpawner?.StartGhostTrail();   // 고스트 트레일 시작
    }

    // 사슬 분리 함수
    public void DetachGrapple()
    {
        if (!IsAttached) return;    // 연결 상태가 아니면 무시

        IsAttached = false;         // 연결 상태 해제
        isSwinging = false;         // 스윙 상태 해제
        swingDirection = 0f;        // 방향 초기화

        joint.enabled = false;      // DistanceJoint 비활성화
        line.enabled = false;       // 라인렌더러 비활성화

        // EnableTrail(false);      // 이동 궤적 비활성화 (현재 주석 처리됨)
        ghostSpawner?.StopGhostTrail();    // 고스트 트레일 정지
    }

    /*
    private void EnableTrail(bool on)
    {
        if (trail == null) return;
        if (on)
        {
            trail.Clear();
            trail.enabled = true;
        }
        else
        {
            trail.enabled = false;
        }
    }
    */
}
