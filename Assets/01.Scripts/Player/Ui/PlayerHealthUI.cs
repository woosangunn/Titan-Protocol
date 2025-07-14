using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 플레이어 체력 정보를 표시하는 UI 전용 컴포넌트
/// - PlayerStatus의 체력 변경 이벤트(OnHealthChanged)를 구독하여 UI 갱신
/// - fillAmount 방식의 체력바 이미지와 TMP 텍스트로 체력 수치 표시
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Image hpBarFill;     // 체력바 이미지 (Image type은 Filled여야 함)
    public TMP_Text hpText;     // 체력 수치를 보여주는 텍스트 (예: "HP 80 / 100")

    private PlayerStatus status;    // 플레이어 상태 컴포넌트 참조 (체력 정보 제공)

    void Awake()
    {
        // 씬에서 PlayerStatus 컴포넌트를 자동으로 찾아 참조 연결
        status = Object.FindFirstObjectByType<PlayerStatus>();

        if (status == null)
        {
            Debug.LogError("[PlayerHealthUI] PlayerStatus를 찾지 못했습니다.");
            enabled = false;    // PlayerStatus 없으면 UI 갱신 불가능하므로 비활성화
        }
    }

    void OnEnable()
    {
        if (status != null)
        {
            // PlayerStatus의 체력 변경 이벤트 구독
            status.OnHealthChanged += UpdateUI;

            // 초기 체력 상태를 UI에 즉시 반영
            UpdateUI(status.CurrentHealth, status.MaxHealth);
        }
    }

    void OnDisable()
    {
        if (status != null)
            // 이벤트 구독 해제하여 메모리 누수 방지
            status.OnHealthChanged -= UpdateUI;
    }

    /// <summary>
    /// 체력 변경 시 호출되는 콜백 함수
    /// - 체력바 fillAmount와 텍스트를 동기화하여 UI 갱신
    /// </summary>
    /// <param name="current">현재 체력</param>
    /// <param name="max">최대 체력</param>
    public void UpdateUI(int current, int max)
    {
        float ratio = max > 0 ? (float)current / max : 0f;

        if (hpBarFill != null)
            hpBarFill.fillAmount = ratio;

        if (hpText != null)
            hpText.text = $"HP {current} / {max}";
    }
}
