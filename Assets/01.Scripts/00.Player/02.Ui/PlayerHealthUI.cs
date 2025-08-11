using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class PlayerHealthUI : MonoBehaviour
{
    public TMP_Text hpText;
    private CanvasGroup canvasGroup;
    private RectTransform rt;

    public Color normalColor = Color.white;
    public Color midColor = Color.red;
    public Color deadColor = Color.black;
    public Color healColor = Color.green;

    public float shakeDurationBase = 0.4f;
    public float shakeIntensityBase = 2f;
    public string glitchChars = "#@!%&$*";
    public float glitchDuration = 0.4f;
    public float glitchInterval = 0.04f;
    public float fadeOutSpeed = 0.8f;
    public float healDuration = 1.2f;

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

        maxHP = 100;
        targetHP = displayHP = maxHP;
        hpText.text = $"HP: {displayHP}";
        hpText.color = normalColor;
        canvasGroup.alpha = 1f;
    }

    void Update()
    {
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

        if (targetHP <= 0)
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime * fadeOutSpeed);
        else
            canvasGroup.alpha = 1f;
    }

    public void UpdateUI(int current, int max)
    {
        maxHP = max;
        targetHP = current;

        float hpRatio = (maxHP > 0) ? (float)current / maxHP : 0f;

        if (current > displayHP)
        {
            hpText.color = healColor;
        }

        if (current < displayHP)
        {
            shakeTimer = shakeDurationBase * (1f - hpRatio);

            if (glitchRoutine != null)
                StopCoroutine(glitchRoutine);
            glitchRoutine = StartCoroutine(GlitchEffectRoutine(current));
        }

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

        if (glitchRoutine == null)
            hpText.text = $"HP: {displayHP}";
    }

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
