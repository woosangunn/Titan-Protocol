using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �÷��̾� ü�� ������ ǥ���ϴ� UI ���� ������Ʈ
/// - PlayerStatus�� ü�� ���� �̺�Ʈ(OnHealthChanged)�� �����Ͽ� UI ����
/// - fillAmount ����� ü�¹� �̹����� TMP �ؽ�Ʈ�� ü�� ��ġ ǥ��
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI ����")]
    public Image hpBarFill;     // ü�¹� �̹��� (Image type�� Filled���� ��)
    public TMP_Text hpText;     // ü�� ��ġ�� �����ִ� �ؽ�Ʈ (��: "HP 80 / 100")

    private PlayerStatus status;    // �÷��̾� ���� ������Ʈ ���� (ü�� ���� ����)

    void Awake()
    {
        // ������ PlayerStatus ������Ʈ�� �ڵ����� ã�� ���� ����
        status = Object.FindFirstObjectByType<PlayerStatus>();

        if (status == null)
        {
            Debug.LogError("[PlayerHealthUI] PlayerStatus�� ã�� ���߽��ϴ�.");
            enabled = false;    // PlayerStatus ������ UI ���� �Ұ����ϹǷ� ��Ȱ��ȭ
        }
    }

    void OnEnable()
    {
        if (status != null)
        {
            // PlayerStatus�� ü�� ���� �̺�Ʈ ����
            status.OnHealthChanged += UpdateUI;

            // �ʱ� ü�� ���¸� UI�� ��� �ݿ�
            UpdateUI(status.CurrentHealth, status.MaxHealth);
        }
    }

    void OnDisable()
    {
        if (status != null)
            // �̺�Ʈ ���� �����Ͽ� �޸� ���� ����
            status.OnHealthChanged -= UpdateUI;
    }

    /// <summary>
    /// ü�� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    /// - ü�¹� fillAmount�� �ؽ�Ʈ�� ����ȭ�Ͽ� UI ����
    /// </summary>
    /// <param name="current">���� ü��</param>
    /// <param name="max">�ִ� ü��</param>
    public void UpdateUI(int current, int max)
    {
        float ratio = max > 0 ? (float)current / max : 0f;

        if (hpBarFill != null)
            hpBarFill.fillAmount = ratio;

        if (hpText != null)
            hpText.text = $"HP {current} / {max}";
    }
}
