using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 체인 게이지 UI 처리
/// - 시작 시 정확한 값(30/30)으로 즉시 표시
/// - 값 변경 시 1씩 빠르게 증감
/// - 게이지 비율에 따라 텍스트 색상이 흰 → 검으로 보간
/// </summary>
public class PlayerChainUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text chainText;

    [Header("애니메이션")]
    public float animSpeed = 0.02f;   // 숫자 1 변화 당 딜레이

    private PlayerGrappleHandler grapple;
    private Coroutine animateRoutine;

    private int displayGauge;
    private int maxGauge;

    void Awake()
    {
        // PlayerGrappleHandler 자동 탐색
        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();
        if (grapple == null)
        {
            Debug.LogError("[PlayerChainUI] PlayerGrappleHandler를 찾을 수 없습니다.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // Awake 이후 정확한 초기 상태로 UI 동기화
        displayGauge = grapple.CurrentGauge;
        maxGauge = grapple.maxGauge;

        SetText(displayGauge, maxGauge); // 즉시 표시

        grapple.OnGaugeChanged += OnGaugeChanged;
    }

    void OnDisable()
    {
        if (grapple != null)
            grapple.OnGaugeChanged -= OnGaugeChanged;
    }

    /// <summary>
    /// PlayerGrappleHandler에서 게이지 변경 이벤트 발생 시 호출
    /// </summary>
    private void OnGaugeChanged(int current, int max)
    {
        maxGauge = max;

        if (animateRoutine != null)
            StopCoroutine(animateRoutine);

        if (current == displayGauge)
        {
            SetText(displayGauge, maxGauge);
            return;
        }

        animateRoutine = StartCoroutine(AnimateGaugeChange(current));
    }

    /// <summary>
    /// 게이지 수치를 1씩 증가/감소시키며 애니메이션 처리
    /// </summary>
    IEnumerator AnimateGaugeChange(int target)
    {
        while (displayGauge != target)
        {
            displayGauge += (displayGauge < target) ? 1 : -1;
            SetText(displayGauge, maxGauge);
            yield return new WaitForSeconds(animSpeed);
        }

        animateRoutine = null;
    }

    /// <summary>
    /// 텍스트 및 색상 보간 적용
    /// </summary>
    private void SetText(int cur, int max)
    {
        chainText.text = $"CHAIN: {cur} / {max}";

        float ratio = (max > 0) ? (float)cur / max : 0f;
        chainText.color = Color.Lerp(Color.black, Color.white, ratio);
    }
}
