using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 탄약 정보를 표시하는 UI 전용 컴포넌트
/// - PlayerStatus의 OnAmmoChanged 이벤트를 구독하여 UI 갱신
/// - 탄약 게이지와 텍스트를 동시에 표시할 수 있음
/// </summary>
public class PlayerAmmoUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Image ammoBarFill;   // 탄약 게이지 바 (Filled 타입 이미지)
    public TMP_Text ammoText;   // 탄약 수치를 표시하는 텍스트 (예: "Ammo 10 / 30")

    private PlayerStatus status;    // 플레이어 상태 컴포넌트 참조 (탄약 정보 제공)

    void Awake()
    {
        // 씬에서 PlayerStatus 컴포넌트를 찾아 참조 연결
        status = Object.FindFirstObjectByType<PlayerStatus>();

        if (status == null)
        {
            Debug.LogError("[PlayerAmmoUI] PlayerStatus를 찾지 못했습니다.");
            enabled = false;    // PlayerStatus 없으면 UI 갱신 불가능하므로 비활성화
        }
    }

    void OnEnable()
    {
        if (status == null) return;

        // PlayerStatus의 탄약 변경 이벤트 구독
        status.OnAmmoChanged += UpdateAmmoUI;

        // 초기 탄약 상태를 UI에 즉시 반영
        UpdateAmmoUI(status.CurrentAmmo, status.MaxAmmo);
    }

    void OnDisable()
    {
        if (status != null)
            // 이벤트 구독 해제하여 메모리 누수 방지
            status.OnAmmoChanged -= UpdateAmmoUI;
    }

    /// <summary>
    /// 탄약 상태 변경 시 호출되는 콜백 함수
    /// - 탄약 게이지와 텍스트를 동기화하여 UI 갱신
    /// </summary>
    /// <param name="current">현재 탄약</param>
    /// <param name="max">최대 탄약</param>
    public void UpdateAmmoUI(int current, int max)
    {
        float ratio = max > 0 ? (float)current / max : 0f;

        if (ammoBarFill != null)
            ammoBarFill.fillAmount = ratio;

        if (ammoText != null)
            ammoText.text = $"Ammo {current} / {max}";
    }
}
