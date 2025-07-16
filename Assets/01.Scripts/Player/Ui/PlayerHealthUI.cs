using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 플레이어 체력 UI 처리 컴포넌트
/// - 글리치 연출, 색상 변화, 흔들림, 페이드아웃, 회복 애니메이션 포함
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text hpText;
    private CanvasGroup canvasGroup;
    private RectTransform rt;

    [Header("색상")]
    public Color normalColor = Color.white;    // 100일 때
    public Color midColor = Color.red;      // 50일 때
    public Color deadColor = Color.black;    // 0일 때
    public Color healColor = Color.green;    // 회복 중

    [Header("연출")]
    public float shakeDurationBase = 0.4f;
    public float shakeIntensityBase = 2f;
    public string glitchChars = "#@!%&$*";
    public float glitchDuration = 0.4f;
    public float glitchInterval = 0.04f;
    public float fadeOutSpeed = 0.8f;
    public float healDuration = 1.2f;

    /* 내부 상태 */
    private int displayHP;
    private int targetHP;
    private int maxHP;

    private float shakeTimer = 0f;
    private Coroutine glitchRoutine;
    private Vector2 originalPos;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rt = hpText.GetComponent<RectTransform>();
        originalPos = rt.anchoredPosition;

        // 시작 시 HP 100 기준으로 초기화
        maxHP = 100;
        targetHP = displayHP = maxHP;
        hpText.text = $"HP: {displayHP}";
        hpText.color = normalColor;
        canvasGroup.alpha = 1f;
    }

    void Update()
    {
        // 흔들림 처리
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float decay = shakeTimer / (shakeDurationBase * (1f - (float)targetHP / maxHP));
            rt.anchoredPosition = originalPos + Random.insideUnitCircle * shakeIntensityBase * decay;
        }
        else
        {
            rt.anchoredPosition = originalPos;
        }

        // 회복 숫자 애니메이션
        if (displayHP < targetHP)
        {
            int step = Mathf.Clamp(
                Mathf.CeilToInt((targetHP - displayHP) * Time.deltaTime / healDuration),
                1, targetHP - displayHP);

            displayHP += step;
            hpText.text = $"HP: {displayHP}";

            if (displayHP == targetHP)
                hpText.color = normalColor;
        }

        // 체력 0이면 서서히 사라짐
        if (targetHP <= 0)
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime * fadeOutSpeed);
        else
            canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// PlayerStatus에서 체력 변경 시 호출
    /// </summary>
    public void UpdateUI(int current, int max)
    {
        maxHP = max;
        targetHP = current;

        float hpRatio = (maxHP > 0) ? (float)current / maxHP : 0f;

        // 회복 중
        if (current > displayHP)
        {
            hpText.color = healColor;
        }

        // 피해 시 글리치 + 흔들림
        if (current < displayHP)
        {
            shakeTimer = shakeDurationBase * (1f - hpRatio);

            if (glitchRoutine != null)
                StopCoroutine(glitchRoutine);
            glitchRoutine = StartCoroutine(GlitchEffectRoutine(current));
        }

        // 색상: 체력 비율 따라 선형 보간
        if (current <= 0)
        {
            hpText.color = deadColor;
        }
        else if (current <= 50)
        {
            hpText.color = Color.Lerp(midColor, deadColor, 1f - hpRatio * 2f);
        }
        else
        {
            hpText.color = Color.Lerp(normalColor, midColor, 1f - (hpRatio - 0.5f) * 2f);
        }

        // 텍스트 즉시 반영 (코루틴 중이면 무시됨)
        if (glitchRoutine == null)
            hpText.text = $"HP: {displayHP}";
    }

    /// <summary>
    /// 피격 시 글리치 텍스트 표시 후 실제 수치로 복원
    /// </summary>
    IEnumerator GlitchEffectRoutine(int finalValue)
    {
        float timer = glitchDuration;
        string finalText = finalValue.ToString();
        char[] buffer = finalText.ToCharArray();

        while (timer > 0f)
        {
            timer -= glitchInterval;

            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = glitchChars[Random.Range(0, glitchChars.Length)];

            hpText.text = "HP: " + new string(buffer);
            yield return new WaitForSeconds(glitchInterval);
        }

        displayHP = finalValue;
        hpText.text = $"HP: {displayHP}";
        glitchRoutine = null;
    }
}
