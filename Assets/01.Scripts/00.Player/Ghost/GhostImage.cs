using UnityEngine;

public class GhostImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private GhostTrailPool pool;

    public float fadeDuration = 0.4f;  // 페이드 아웃 지속 시간
    private float timer;

    /// <summary>
    /// 초기화 함수: 스프라이트, 좌우 반전, 시작 색상, 풀 참조 설정
    /// </summary>
    public void Init(Sprite sprite, bool flipX, Color startColor, GhostTrailPool trailPool)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        sr.sprite = sprite;          // 스프라이트 지정
        sr.flipX = flipX;            // 좌우 반전 여부
        sr.color = startColor;       // 초기 색상 (알파 포함)

        pool = trailPool;            // 풀 참조 저장
        timer = 0f;                  // 타이머 초기화
        gameObject.SetActive(true);  // 게임 오브젝트 활성화
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 알파값을 0.6 → 0으로 선형 보간 (페이드 아웃 효과)
        float alpha = Mathf.Lerp(0.6f, 0f, timer / fadeDuration);

        if (sr != null)
        {
            // 현재 색상 유지하면서 알파값만 변경
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        }

        // 페이드 아웃 완료 시
        if (timer >= fadeDuration)
        {
            gameObject.SetActive(false);   // 비활성화
            pool.Return(gameObject);        // 풀에 반환하여 재사용 대기
        }
    }
}
