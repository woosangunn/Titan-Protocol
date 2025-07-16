using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// �÷��̾� ź�� UI ǥ�� ������Ʈ
/// - ź�� ���� �� �ؽ�Ʈ ��ġ ���� �ִϸ��̼� ����
/// - ź�� ���� ���� ���� ������ ��� �� ���������� ���� ��ο���
/// </summary>
public class PlayerAmmoUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text ammoText;         // "AMMO: 10 / 30" ���� �ؽ�Ʈ

    private PlayerStatus status;
    private Coroutine animate;
    private int displayAmmo;          // ���� ǥ�õǴ� ź�� ��
    private int maxAmmo;

    [Header("����")]
    public float animSpeed = 0.05f;   // ���� �ϳ� ��ȭ �� ������ (�������� ����)

    void Awake()
    {
        status = Object.FindFirstObjectByType<PlayerStatus>();
        if (status == null)
        {
            Debug.LogError("[PlayerAmmoUI] PlayerStatus�� ã�� ���߽��ϴ�.");
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        status.OnAmmoChanged += UpdateAmmoUI;

        // �ʱ� �� ��� �ݿ� (���� �� 30 / 30ó��)
        displayAmmo = status.CurrentAmmo;
        maxAmmo = status.MaxAmmo;
        SetText(displayAmmo, maxAmmo);
    }

    void OnDisable()
    {
        if (status != null)
            status.OnAmmoChanged -= UpdateAmmoUI;
    }

    /// <summary>
    /// PlayerStatus�� OnAmmoChanged �̺�Ʈ�κ��� ȣ���
    /// </summary>
    public void UpdateAmmoUI(int current, int max)
    {
        maxAmmo = max;

        if (animate != null)
            StopCoroutine(animate);

        if (current == displayAmmo)
        {
            // ���� ���� ����
            SetText(displayAmmo, maxAmmo);
            return;
        }

        animate = StartCoroutine(AnimateAmmoChange(current));
    }

    /// <summary>
    /// ź�� ��ġ�� ����� �� ���ڰ� �ϳ��� ������ �����ϵ��� �ִϸ��̼� ó��
    /// </summary>
    IEnumerator AnimateAmmoChange(int target)
    {
        while (displayAmmo != target)
        {
            displayAmmo += (displayAmmo < target) ? 1 : -1;
            SetText(displayAmmo, maxAmmo);
            yield return new WaitForSeconds(animSpeed);
        }

        animate = null;
    }

    /// <summary>
    /// �ؽ�Ʈ ǥ�� �� ���� ���� ó��
    /// </summary>
    private void SetText(int current, int max)
    {
        ammoText.text = $"AMMO: {current} / {max}";

        float ratio = (max > 0) ? (float)current / max : 0f;
        ammoText.color = Color.Lerp(Color.black, Color.white, ratio);
    }
}
