using UnityEngine;

/// <summary>
/// UI ��� ������ �ϴ� ������Ʈ
/// - PlayerStatus�� PlayerGrappleHandler�� �����Ͽ� UI ������Ʈ�� ���� ����
/// - ���� UI ���� ������ ������ �ʰ�, ���� ���� �˸��� ���Ḹ ����
/// </summary>
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerHealthUI playerHealthUI;   // ü�� UI ������Ʈ ����
    [SerializeField] private PlayerChainUI playerChainUI;     // �罽 UI ������Ʈ ����
    [SerializeField] private PlayerAmmoUI playerAmmoUI;       // ź�� UI ������Ʈ ����

    private PlayerStatus playerStatus;             // �÷��̾� ���� ������Ʈ ����
    private PlayerGrappleHandler playerGrapple;   // �罽 ���� ������Ʈ ����

    private void Awake()
    {
        // ������ PlayerStatus, PlayerGrappleHandler �ڵ� Ž�� �� ����
        playerStatus = Object.FindFirstObjectByType<PlayerStatus>();
        playerGrapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();

        // �ʼ� ������Ʈ ���� �� ���� ��� �� ��Ȱ��ȭ
        if (playerStatus == null || playerGrapple == null)
        {
            Debug.LogError("[PlayerUI] �ʼ� ������Ʈ�� �����ϴ�.");
            enabled = false;
            return;
        }

        // PlayerStatus�� ü��, ź�� ���� �̺�Ʈ ����
        playerStatus.OnHealthChanged += OnHealthChanged;
        playerStatus.OnAmmoChanged += OnAmmoChanged;

        // �ʱ� UI ���� ����ȭ (���� ü��, ź�� ���� UI�� ����)
        OnHealthChanged(playerStatus.CurrentHealth, playerStatus.MaxHealth);
        OnAmmoChanged(playerStatus.CurrentAmmo, playerStatus.MaxAmmo);
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ���� (�޸� ���� ����)
        if (playerStatus != null)
        {
            playerStatus.OnHealthChanged -= OnHealthChanged;
            playerStatus.OnAmmoChanged -= OnAmmoChanged;
        }
    }

    private void Update()
    {
        // �罽 UI�� ���� Update()�� ���� ó���ϹǷ� ���� ȣ�� ����
        // �ʿ� �� playerChainUI ������Ʈ ȣ�� ����
    }

    /// <summary>
    /// PlayerStatus���� ü�� ���� �̺�Ʈ �߻� �� ȣ��
    /// - PlayerHealthUI�� UpdateUI �޼��带 ȣ���Ͽ� ȭ�� ����
    /// </summary>
    private void OnHealthChanged(int current, int max)
    {
        playerHealthUI?.UpdateUI(current, max);
    }

    /// <summary>
    /// PlayerStatus���� ź�� ���� �̺�Ʈ �߻� �� ȣ��
    /// - PlayerAmmoUI�� UpdateAmmoUI �޼��带 ȣ���Ͽ� ȭ�� ����
    /// </summary>
    private void OnAmmoChanged(int current, int max)
    {
        playerAmmoUI?.UpdateAmmoUI(current, max);
    }
}
