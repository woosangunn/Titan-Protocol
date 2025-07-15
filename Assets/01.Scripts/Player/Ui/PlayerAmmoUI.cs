using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerAmmoUI : MonoBehaviour
{
    [Header("UI ����")]
    public TMP_Text ammoText;

    private PlayerStatus status;
    private Coroutine animateCoroutine;
    private int displayAmmo = 0;  // UI�� �������� ź�� ��
    private int maxAmmo = 0;

    private float reloadAnimationSpeed = 0.05f; // 1�ߴ� �ִϸ��̼� ���� (���� ����)

    void Awake()
    {
        status = Object.FindFirstObjectByType<PlayerStatus>();

        if (status == null)
        {
            Debug.LogError("[PlayerAmmoUI] PlayerStatus�� ã�� ���߽��ϴ�.");
            enabled = false;
        }
    }

    void OnEnable()
    {
        if (status == null) return;

        status.OnAmmoChanged += UpdateAmmoUI;
        UpdateAmmoUI(status.CurrentAmmo, status.MaxAmmo);
    }

    void OnDisable()
    {
        if (status != null)
            status.OnAmmoChanged -= UpdateAmmoUI;
    }

    /// <summary>
    /// PlayerStatus���� ȣ���
    /// </summary>
    public void UpdateAmmoUI(int current, int max)
    {
        maxAmmo = max;

        // ��ǥ ź���� �̹� ǥ�� ���� ��ġ�� ������ �ִϸ��̼� �ߺ� ����
        if (animateCoroutine != null && current == displayAmmo)
            return;

        // ���� �ִϸ��̼��� �ִٸ� ����
        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);

        // 1�߾� �ִϸ��̼����� ���� ��ȭ
        animateCoroutine = StartCoroutine(AnimateAmmo(displayAmmo, current, max));
    }

    private IEnumerator AnimateAmmo(int from, int to, int max)
    {
        int current = from;

        while (current != to)
        {
            if (to > current)
                current++;
            else
                current--;  // ź�� ���� �� �ִϸ��̼ǵ� �ε巴�� ���� ����

            displayAmmo = current;
            SetAmmoDisplay(displayAmmo, max);

            yield return new WaitForSeconds(reloadAnimationSpeed);
        }

        animateCoroutine = null;
    }

    private void SetAmmoDisplay(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo {current} / {max}";
            ammoText.color = GetAmmoColor(current, max);
        }
    }

    private Color GetAmmoColor(int current, int max)
    {
        if (max <= 0) return Color.black;

        float ratio = Mathf.Clamp01((float)current / max);
        return Color.Lerp(Color.black, Color.white, ratio);
    }

    /// <summary>
    /// �ܺο��� ���� �ӵ��� ������ �� �ִ� �Լ� (�ɼ�)
    /// </summary>
    public void SetReloadAnimationSpeed(float secondsPerAmmo)
    {
        reloadAnimationSpeed = secondsPerAmmo;
    }
}
