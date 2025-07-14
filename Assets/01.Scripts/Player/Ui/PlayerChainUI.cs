using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �罽 ���� �� ���� �ð��� ǥ���ϴ� UI ���� ������Ʈ
/// - PlayerGrappleHandler�� ���¸� ���� �о� ������ �� �ؽ�Ʈ ������Ʈ
/// - �罽�� ����Ǿ� ���� ���� UI�� Ȱ��ȭ�Ͽ� ǥ��
/// </summary>
public class PlayerChainUI : MonoBehaviour
{
    [Header("UI ����")]
    public Image chainBarFill;     // ������ �� �̹��� (Filled Ÿ��)
    public TMP_Text chainText;     // ���� �ð� ǥ�ÿ� �ؽ�Ʈ

    private PlayerGrappleHandler grapple;    // �罽 ���� ������Ʈ ����

    void Awake()
    {
        // ������ PlayerGrappleHandler ������Ʈ�� ã�� ���� ����
        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();

        if (grapple == null)
        {
            Debug.LogError("[PlayerChainUI] PlayerGrappleHandler�� ã�� ���߽��ϴ�.");
            enabled = false;    // �罽 ���� ���¸� �� �� ������ ��Ȱ��ȭ
        }
    }

    void Update()
    {
        if (grapple == null) return;

        if (grapple.IsAttached)
        {
            // �罽�� ����Ǿ� ���� �� ���� �ð��� �ִ� �ð��� ������
            float remain = grapple.RemainingChainTime();
            float max = grapple.maxAttachTime;
            float ratio = max > 0f ? remain / max : 0f;

            // ������ �� fillAmount�� �ؽ�Ʈ ������Ʈ
            if (chainBarFill != null)
                chainBarFill.fillAmount = ratio;

            if (chainText != null)
                chainText.text = $"Chain: {remain:F1}s / {max:F1}s";
        }
        else
        {
            // �罽�� ������� ���� ���¿����� ������ �ʱ�ȭ �� �ؽ�Ʈ ǥ��
            if (chainBarFill != null)
                chainBarFill.fillAmount = 0f;

            if (chainText != null)
                chainText.text = "Chain: -";
        }
    }
}
