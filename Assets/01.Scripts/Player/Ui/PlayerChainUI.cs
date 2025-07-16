using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// ü�� ������ UI ó��
/// - ���� �� ��Ȯ�� ��(30/30)���� ��� ǥ��
/// - �� ���� �� 1�� ������ ����
/// - ������ ������ ���� �ؽ�Ʈ ������ �� �� ������ ����
/// </summary>
public class PlayerChainUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text chainText;

    [Header("�ִϸ��̼�")]
    public float animSpeed = 0.02f;   // ���� 1 ��ȭ �� ������

    private PlayerGrappleHandler grapple;
    private Coroutine animateRoutine;

    private int displayGauge;
    private int maxGauge;

    void Awake()
    {
        // PlayerGrappleHandler �ڵ� Ž��
        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();
        if (grapple == null)
        {
            Debug.LogError("[PlayerChainUI] PlayerGrappleHandler�� ã�� �� �����ϴ�.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // Awake ���� ��Ȯ�� �ʱ� ���·� UI ����ȭ
        displayGauge = grapple.CurrentGauge;
        maxGauge = grapple.maxGauge;

        SetText(displayGauge, maxGauge); // ��� ǥ��

        grapple.OnGaugeChanged += OnGaugeChanged;
    }

    void OnDisable()
    {
        if (grapple != null)
            grapple.OnGaugeChanged -= OnGaugeChanged;
    }

    /// <summary>
    /// PlayerGrappleHandler���� ������ ���� �̺�Ʈ �߻� �� ȣ��
    /// </summary>
    private void OnGaugeChanged(int current, int max)
    {
        maxGauge = max;

        if (animateRoutine != null)
            StopCoroutine(animateRoutine);

        if (current == displayGauge)
        {
            SetText(displayGauge, maxGauge);
            return;
        }

        animateRoutine = StartCoroutine(AnimateGaugeChange(current));
    }

    /// <summary>
    /// ������ ��ġ�� 1�� ����/���ҽ�Ű�� �ִϸ��̼� ó��
    /// </summary>
    IEnumerator AnimateGaugeChange(int target)
    {
        while (displayGauge != target)
        {
            displayGauge += (displayGauge < target) ? 1 : -1;
            SetText(displayGauge, maxGauge);
            yield return new WaitForSeconds(animSpeed);
        }

        animateRoutine = null;
    }

    /// <summary>
    /// �ؽ�Ʈ �� ���� ���� ����
    /// </summary>
    private void SetText(int cur, int max)
    {
        chainText.text = $"CHAIN: {cur} / {max}";

        float ratio = (max > 0) ? (float)cur / max : 0f;
        chainText.color = Color.Lerp(Color.black, Color.white, ratio);
    }
}
