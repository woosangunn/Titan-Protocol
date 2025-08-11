using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerAmmoUI : MonoBehaviour
{
    public TMP_Text ammoText;

    private PlayerStatus status;
    private Coroutine animate;
    private int displayAmmo;
    private int maxAmmo;

    public float animSpeed = 0.05f;

    void Awake()
    {
        status = Object.FindFirstObjectByType<PlayerStatus>();
        if (status == null)
        {
            Debug.LogError("[PlayerAmmoUI] PlayerStatus를 찾지 못했습니다.");
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        status.OnAmmoChanged += UpdateAmmoUI;

        displayAmmo = status.CurrentAmmo;
        maxAmmo = status.MaxAmmo;
        SetText(displayAmmo, maxAmmo);
    }

    void OnDisable()
    {
        if (status != null)
            status.OnAmmoChanged -= UpdateAmmoUI;
    }

    public void UpdateAmmoUI(int current, int max)
    {
        maxAmmo = max;

        if (animate != null)
            StopCoroutine(animate);

        if (current == displayAmmo)
        {
            SetText(displayAmmo, maxAmmo);
            return;
        }

        animate = StartCoroutine(AnimateAmmoChange(current));
    }

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

    private void SetText(int current, int max)
    {
        ammoText.text = $"AMMO: {current} / {max}";

        float ratio = (max > 0) ? (float)current / max : 0f;
        ammoText.color = Color.Lerp(Color.black, Color.white, ratio);
    }
}
