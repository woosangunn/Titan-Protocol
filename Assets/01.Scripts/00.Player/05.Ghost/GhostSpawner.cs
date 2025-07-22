using UnityEngine;
using System.Collections;

public class GhostSpawner : MonoBehaviour
{
    [Tooltip("재사용할 유령 이미지 풀")]
    public GhostTrailPool trailPool;           // 재사용할 유령 이미지 풀
    [Tooltip("유령 이미지를 복사할 원본 스프라이트 렌더러")]
    public SpriteRenderer sourceRenderer;      // 유령 이미지를 복사할 원본 스프라이트 렌더러
    [Tooltip("유령 이미지 생성 간격")]
    public float spawnInterval = 0.05f;        // 유령 이미지 생성 간격
    [Tooltip("유령 이미지 페이드 지속 시간 (GhostImage에서 사용)")]
    public float fadeDuration = 0.3f;          // 유령 이미지 페이드 지속 시간 (GhostImage에서 사용)

    void Awake()
    {
        // sourceRenderer 미지정 시 자식에서 자동 탐색
        if (sourceRenderer == null)
            sourceRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // 유령 트레일 생성 시작
    public void StartGhostTrail() => StartCoroutine(SpawnTrail());

    // 유령 트레일 생성 정지
    public void StopGhostTrail() => StopAllCoroutines();

    IEnumerator SpawnTrail()
    {
        while (true)
        {
            // 필요 컴포넌트 누락 시 코루틴 종료
            if (sourceRenderer == null || trailPool == null) yield break;

            // 풀에서 유령 이미지 오브젝트 획득
            GameObject ghost = trailPool.Get();

            // 현재 위치 및 회전으로 배치
            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;

            // GhostImage 컴포넌트 초기화 (스프라이트, 뒤집기, 투명도, 풀 참조)
            GhostImage ghostComp = ghost.GetComponent<GhostImage>();
            if (ghostComp != null)
            {
                ghostComp.Init(
                    sourceRenderer.sprite,
                    sourceRenderer.flipX,
                    new Color(1f, 1f, 1f, 0.6f),
                    trailPool
                );
            }

            // 다음 생성까지 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
