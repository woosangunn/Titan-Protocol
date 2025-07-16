using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerChainUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text chainText;    // "CHAIN: 25 / 30"
    public float animSpeed = 0.05f;

    private PlayerGrappleHandler grapple;
    private Coroutine animate;
    private int displayGauge;
    private int maxGauge;

    void Awake()
    {
        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();
        if (grapple == null) { Debug.LogError("[ChainUI] GrappleHandler 없음"); enabled = false; return; }
    }

    void OnEnable()
    {
        grapple.OnGaugeChanged += UpdateGauge;
        displayGauge = grapple.CurrentGauge;
        maxGauge = grapple.maxGauge;
        SetText(displayGauge, maxGauge);
    }

    void OnDisable()
    {
        if (grapple != null) grapple.OnGaugeChanged -= UpdateGauge;
    }

    /* 이벤트 콜백 */
    private void UpdateGauge(int cur, int max)
    {
        maxGauge = max;
        if (animate != null) StopCoroutine(animate);
        animate = StartCoroutine(AnimateGauge(cur));
    }

    IEnumerator AnimateGauge(int target)
    {
        while (displayGauge != target)
        {
            displayGauge += (displayGauge < target) ? 1 : -1;
            SetText(displayGauge, maxGauge);
            yield return new WaitForSeconds(animSpeed);
        }
        animate = null;
    }

    private void SetText(int cur, int max)
    {
        chainText.text = $"CHAIN: {cur} / {max}";
        float ratio = (max > 0) ? (float)cur / max : 0f;
        chainText.color = Color.Lerp(Color.black, Color.white, ratio); // 흰 → 검
    }
}
