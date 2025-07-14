using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ź�� ������ ǥ���ϴ� UI ���� ������Ʈ
/// - PlayerStatus�� OnAmmoChanged �̺�Ʈ�� �����Ͽ� UI ����
/// - ź�� �������� �ؽ�Ʈ�� ���ÿ� ǥ���� �� ����
/// </summary>
public class PlayerAmmoUI : MonoBehaviour
{
    [Header("UI ����")]
    public Image ammoBarFill;   // ź�� ������ �� (Filled Ÿ�� �̹���)
    public TMP_Text ammoText;   // ź�� ��ġ�� ǥ���ϴ� �ؽ�Ʈ (��: "Ammo 10 / 30")

    private PlayerStatus status;    // �÷��̾� ���� ������Ʈ ���� (ź�� ���� ����)

    void Awake()
    {
        // ������ PlayerStatus ������Ʈ�� ã�� ���� ����
        status = Object.FindFirstObjectByType<PlayerStatus>();

        if (status == null)
        {
            Debug.LogError("[PlayerAmmoUI] PlayerStatus�� ã�� ���߽��ϴ�.");
            enabled = false;    // PlayerStatus ������ UI ���� �Ұ����ϹǷ� ��Ȱ��ȭ
        }
    }

    void OnEnable()
    {
        if (status == null) return;

        // PlayerStatus�� ź�� ���� �̺�Ʈ ����
        status.OnAmmoChanged += UpdateAmmoUI;

        // �ʱ� ź�� ���¸� UI�� ��� �ݿ�
        UpdateAmmoUI(status.CurrentAmmo, status.MaxAmmo);
    }

    void OnDisable()
    {
        if (status != null)
            // �̺�Ʈ ���� �����Ͽ� �޸� ���� ����
            status.OnAmmoChanged -= UpdateAmmoUI;
    }

    /// <summary>
    /// ź�� ���� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    /// - ź�� �������� �ؽ�Ʈ�� ����ȭ�Ͽ� UI ����
    /// </summary>
    /// <param name="current">���� ź��</param>
    /// <param name="max">�ִ� ź��</param>
    public void UpdateAmmoUI(int current, int max)
    {
        float ratio = max > 0 ? (float)current / max : 0f;

        if (ammoBarFill != null)
            ammoBarFill.fillAmount = ratio;

        if (ammoText != null)
            ammoText.text = $"Ammo {current} / {max}";
    }
}
