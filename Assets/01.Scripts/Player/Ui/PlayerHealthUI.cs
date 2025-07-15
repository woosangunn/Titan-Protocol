using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text hpText;
    public TMP_Text glitchText;

    [Header("효과 설정")]
    public float baseShakeIntensity = 2f;
    public float flashDuration = 5f;

    [Header("글리치 설정")]
    public float glitchDurationDamage = 0.05f;
    public float glitchDurationHeal = 0.05f;
    public float minLowHpGlitchChance = 0.0005f;
    public float maxLowHpGlitchChance = 0.0001f;
    public string glitchChars = "#@!%&$*";

    private int currentHP;
    private int maxHP;

    private float flashTimer = 0f;
    private Color baseColor = Color.white;
    private Color flashColor = Color.white;

    private RectTransform rectTransform;
    private RectTransform glitchRect;
    private Vector2 originalPos;
    private Coroutine glitchCoroutine;

    void Awake()
    {
        rectTransform = hpText.GetComponent<RectTransform>();
        glitchRect = glitchText.GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;

        glitchText.gameObject.SetActive(false);
        hpText.text = "";
    }

    void Update()
    {
        UpdateShake();
        UpdateFlash();
        TryGlitchWhenLowHP();
    }

    // 외부에서 체력 업데이트할 때 호출됨
    public void UpdateUI(int current, int max)
    {
        currentHP = current;
        maxHP = max;

        SetText();
        SetColor();
        StartFlashEffect();

        // 글리치 효과 (기존 코루틴 멈추고 새로 실행)
        if (glitchCoroutine != null) StopCoroutine(glitchCoroutine);
        float duration = current < currentHP ? glitchDurationDamage : glitchDurationHeal;
        if (current != currentHP)
            glitchCoroutine = StartCoroutine(GlitchOverlayEffect(duration));
    }

    private void SetText()
    {
        hpText.text = $"HP: {currentHP} / {maxHP}";
    }

    private void SetColor()
    {
        baseColor = currentHP <= 0 ? Color.black : Color.white;
    }

    private void StartFlashEffect()
    {
        flashTimer = flashDuration * (1f - Mathf.Clamp01((float)currentHP / maxHP));
        flashColor = currentHP < maxHP ? Color.red : Color.green;
    }

    private void UpdateFlash()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            float t = flashTimer / flashDuration;
            hpText.color = Color.Lerp(baseColor, flashColor, t);
        }
        else
        {
            hpText.color = baseColor;
        }
    }

    private void UpdateShake()
    {
        if (currentHP <= 80)
        {
            float hpRatio = Mathf.Clamp01((float)currentHP / 80f);
            float intensity = baseShakeIntensity * (1f - hpRatio);
            rectTransform.anchoredPosition = originalPos + Random.insideUnitCircle * intensity;
        }
        else
        {
            rectTransform.anchoredPosition = originalPos;
        }
    }

    private void TryGlitchWhenLowHP()
    {
        if (currentHP <= 50 && glitchCoroutine == null)
        {
            float hpRatio = Mathf.Clamp01((float)currentHP / 50f);
            float chance = Mathf.Lerp(maxLowHpGlitchChance, minLowHpGlitchChance, hpRatio);
            if (Random.value < chance)
            {
                glitchCoroutine = StartCoroutine(GlitchOverlayEffect(0.2f));
            }
        }
    }

    private IEnumerator GlitchOverlayEffect(float duration)
    {
        hpText.gameObject.SetActive(false);
        glitchText.gameObject.SetActive(true);

        Vector2 glitchOriginalPos = glitchRect.anchoredPosition;
        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            string glitch = "";
            int length = Mathf.Max(2, currentHP.ToString().Length);
            for (int i = 0; i < length; i++)
                glitch += glitchChars[Random.Range(0, glitchChars.Length)];

            glitchText.text = glitch;
            glitchText.alpha = Random.Range(0.6f, 1f);

            float hpRatio = Mathf.Clamp01((float)currentHP / 80f);
            float intensity = baseShakeIntensity * 1.5f * (1f - hpRatio);
            glitchRect.anchoredPosition = glitchOriginalPos + Random.insideUnitCircle * intensity;

            yield return new WaitForSeconds(0.05f);
        }

        glitchText.gameObject.SetActive(false);
        glitchRect.anchoredPosition = glitchOriginalPos;
        hpText.gameObject.SetActive(true);
        glitchCoroutine = null;
    }
}
