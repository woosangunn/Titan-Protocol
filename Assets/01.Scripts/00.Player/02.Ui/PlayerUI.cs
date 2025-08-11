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
            Debug.LogError("[PlayerUI] �ʼ� ������Ʈ�� �����ϴ�.");
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
        // �罽 UI�� ���� Update()�� ���� ó���ϹǷ� ���� ȣ�� ����
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
