using UnityEngine;

/// <summary>
/// UI 허브 역할을 하는 컴포넌트
/// - PlayerStatus와 PlayerGrappleHandler를 구독하여 UI 컴포넌트에 상태 전달
/// - 직접 UI 내부 로직을 가지지 않고, 상태 변경 알림을 연결만 수행
/// </summary>
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerHealthUI playerHealthUI;   // 체력 UI 컴포넌트 참조
    [SerializeField] private PlayerChainUI playerChainUI;     // 사슬 UI 컴포넌트 참조
    [SerializeField] private PlayerAmmoUI playerAmmoUI;       // 탄약 UI 컴포넌트 참조

    private PlayerStatus playerStatus;             // 플레이어 상태 컴포넌트 참조
    private PlayerGrappleHandler playerGrapple;   // 사슬 조작 컴포넌트 참조

    private void Awake()
    {
        // 씬에서 PlayerStatus, PlayerGrappleHandler 자동 탐색 및 참조
        playerStatus = Object.FindFirstObjectByType<PlayerStatus>();
        playerGrapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();

        // 필수 컴포넌트 누락 시 오류 출력 및 비활성화
        if (playerStatus == null || playerGrapple == null)
        {
            Debug.LogError("[PlayerUI] 필수 컴포넌트가 없습니다.");
            enabled = false;
            return;
        }

        // PlayerStatus의 체력, 탄약 변경 이벤트 구독
        playerStatus.OnHealthChanged += OnHealthChanged;
        playerStatus.OnAmmoChanged += OnAmmoChanged;

        // 초기 UI 상태 동기화 (현재 체력, 탄약 값을 UI에 전달)
        OnHealthChanged(playerStatus.CurrentHealth, playerStatus.MaxHealth);
        OnAmmoChanged(playerStatus.CurrentAmmo, playerStatus.MaxAmmo);
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        if (playerStatus != null)
        {
            playerStatus.OnHealthChanged -= OnHealthChanged;
            playerStatus.OnAmmoChanged -= OnAmmoChanged;
        }
    }

    private void Update()
    {
        // 사슬 UI는 내부 Update()로 직접 처리하므로 별도 호출 없음
        // 필요 시 playerChainUI 업데이트 호출 가능
    }

    /// <summary>
    /// PlayerStatus에서 체력 변경 이벤트 발생 시 호출
    /// - PlayerHealthUI의 UpdateUI 메서드를 호출하여 화면 갱신
    /// </summary>
    private void OnHealthChanged(int current, int max)
    {
        playerHealthUI?.UpdateUI(current, max);
    }

    /// <summary>
    /// PlayerStatus에서 탄약 변경 이벤트 발생 시 호출
    /// - PlayerAmmoUI의 UpdateAmmoUI 메서드를 호출하여 화면 갱신
    /// </summary>
    private void OnAmmoChanged(int current, int max)
    {
        playerAmmoUI?.UpdateAmmoUI(current, max);
    }
}
