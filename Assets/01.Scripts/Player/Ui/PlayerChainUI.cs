using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerChainUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Image chainBarFill;     // 게이지 바 이미지 (Filled 타입)
    public TMP_Text chainText;     // 남은 시간 표시용 텍스트

    private PlayerGrappleHandler grapple;

    void Awake()
    {
        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();

        if (grapple == null)
        {
            Debug.LogError("[PlayerChainUI] PlayerGrappleHandler를 찾지 못했습니다.");
            enabled = false;
        }
    }

    void Update()
    {
        if (grapple == null) return;

        float remain = grapple.RemainingChainTime();
        float max = grapple.maxAttachTime;
        float ratio = max > 0f ? remain / max : 0f;

        // 게이지 채우기 (체인 연결 여부와 상관없이 항상 표시)
        if (chainBarFill != null)
            chainBarFill.fillAmount = ratio;

        // 텍스트도 항상 표시하되 남은 시간이 0이면 "Chain: Empty" 등으로 표시
        if (chainText != null)
        {
            if (remain <= 0f)
                chainText.text = "Chain: Empty";
            else
                chainText.text = $"Chain: {remain:F1}s / {max:F1}s";
        }
    }
}
