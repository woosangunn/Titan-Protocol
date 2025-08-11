using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerChainUI : MonoBehaviour
{
    public TMP_Text chainText;
    public float animSpeed = 0.02f;

    private PlayerGrappleHandler grapple;
    private Coroutine animateRoutine;

    private int displayGauge;
    private int maxGauge;

    void Awake()
    {
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
        displayGauge = grapple.CurrentGauge;
        maxGauge = grapple.maxGauge;

        SetText(displayGauge, maxGauge);

        grapple.OnGaugeChanged += OnGaugeChanged;
    }

    void OnDisable()
    {
        if (grapple != null)
            grapple.OnGaugeChanged -= OnGaugeChanged;
    }

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

    private void SetText(int cur, int max)
    {
        chainText.text = $"CHAIN: {cur} / {max}";

        float ratio = (max > 0) ? (float)cur / max : 0f;
        chainText.color = Color.Lerp(Color.black, Color.white, ratio);
    }
}
