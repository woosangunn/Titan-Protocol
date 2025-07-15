using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerChainUI : MonoBehaviour
{
    [Header("UI ����")]
    public Image chainBarFill;     // ������ �� �̹��� (Filled Ÿ��)
    public TMP_Text chainText;     // ���� �ð� ǥ�ÿ� �ؽ�Ʈ

    private PlayerGrappleHandler grapple;

    void Awake()
    {
        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();

        if (grapple == null)
        {
            Debug.LogError("[PlayerChainUI] PlayerGrappleHandler�� ã�� ���߽��ϴ�.");
            enabled = false;
        }
    }

    void Update()
    {
        if (grapple == null) return;

        float remain = grapple.RemainingChainTime();
        float max = grapple.maxAttachTime;
        float ratio = max > 0f ? remain / max : 0f;

        // ������ ä��� (ü�� ���� ���ο� ������� �׻� ǥ��)
        if (chainBarFill != null)
            chainBarFill.fillAmount = ratio;

        // �ؽ�Ʈ�� �׻� ǥ���ϵ� ���� �ð��� 0�̸� "Chain: Empty" ������ ǥ��
        if (chainText != null)
        {
            if (remain <= 0f)
                chainText.text = "Chain: Empty";
            else
                chainText.text = $"Chain: {remain:F1}s / {max:F1}s";
        }
    }
}
