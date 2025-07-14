using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 사슬 연결 시 남은 시간을 표시하는 UI 전용 컴포넌트
/// - PlayerGrappleHandler의 상태를 직접 읽어 게이지 및 텍스트 업데이트
/// - 사슬이 연결되어 있을 때만 UI를 활성화하여 표시
/// </summary>
public class PlayerChainUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Image chainBarFill;     // 게이지 바 이미지 (Filled 타입)
    public TMP_Text chainText;     // 남은 시간 표시용 텍스트

    private PlayerGrappleHandler grapple;    // 사슬 조작 컴포넌트 참조

    void Awake()
    {
        // 씬에서 PlayerGrappleHandler 컴포넌트를 찾아 참조 연결
        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();

        if (grapple == null)
        {
            Debug.LogError("[PlayerChainUI] PlayerGrappleHandler를 찾지 못했습니다.");
            enabled = false;    // 사슬 관련 상태를 알 수 없으면 비활성화
        }
    }

    void Update()
    {
        if (grapple == null) return;

        if (grapple.IsAttached)
        {
            // 사슬이 연결되어 있을 때 남은 시간과 최대 시간을 가져옴
            float remain = grapple.RemainingChainTime();
            float max = grapple.maxAttachTime;
            float ratio = max > 0f ? remain / max : 0f;

            // 게이지 바 fillAmount와 텍스트 업데이트
            if (chainBarFill != null)
                chainBarFill.fillAmount = ratio;

            if (chainText != null)
                chainText.text = $"Chain: {remain:F1}s / {max:F1}s";
        }
        else
        {
            // 사슬이 연결되지 않은 상태에서는 게이지 초기화 및 텍스트 표시
            if (chainBarFill != null)
                chainBarFill.fillAmount = 0f;

            if (chainText != null)
                chainText.text = "Chain: -";
        }
    }
}
