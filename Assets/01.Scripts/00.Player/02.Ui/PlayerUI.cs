using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerHealthUI playerHealthUI;
    [SerializeField] private PlayerChainUI playerChainUI;
    [SerializeField] private PlayerAmmoUI playerAmmoUI;

    private PlayerStatus playerStatus;
    private PlayerGrappleHandler playerGrapple;

    private void Awake()
    {
        playerStatus = Object.FindFirstObjectByType<PlayerStatus>();
        playerGrapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();

        if (playerStatus == null || playerGrapple == null)
        {
            Debug.LogError("[PlayerUI] 필수 컴포넌트가 없습니다.");
            enabled = false;
            return;
        }

        playerStatus.OnHealthChanged += OnHealthChanged;
        playerStatus.OnAmmoChanged += OnAmmoChanged;

        OnHealthChanged(playerStatus.CurrentHealth, playerStatus.MaxHealth);
        OnAmmoChanged(playerStatus.CurrentAmmo, playerStatus.MaxAmmo);
    }

    private void OnDestroy()
    {
        if (playerStatus != null)
        {
            playerStatus.OnHealthChanged -= OnHealthChanged;
            playerStatus.OnAmmoChanged -= OnAmmoChanged;
        }
    }

    private void Update()
    {
        // 사슬 UI는 내부 Update()로 직접 처리하므로 별도 호출 없음
    }

    private void OnHealthChanged(int current, int max)
    {
        playerHealthUI?.UpdateUI(current, max);
    }

    private void OnAmmoChanged(int current, int max)
    {
        playerAmmoUI?.UpdateAmmoUI(current, max);
    }
}
