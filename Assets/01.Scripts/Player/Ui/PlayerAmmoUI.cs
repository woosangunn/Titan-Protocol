using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerAmmoUI : MonoBehaviour
{
    [Header("UI 참조")]
    public TMP_Text ammoText;

    private PlayerStatus status;
    private Coroutine animateCoroutine;
    private int displayAmmo = 0;  // UI에 보여지는 탄약 수
    private int maxAmmo = 0;

    private float reloadAnimationSpeed = 0.05f; // 1발당 애니메이션 간격 (변경 가능)

    void Awake()
    {
        status = Object.FindFirstObjectByType<PlayerStatus>();

        if (status == null)
        {
            Debug.LogError("[PlayerAmmoUI] PlayerStatus를 찾지 못했습니다.");
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
    /// PlayerStatus에서 호출됨
    /// </summary>
    public void UpdateAmmoUI(int current, int max)
    {
        maxAmmo = max;

        // 목표 탄약이 이미 표시 중인 수치와 같으면 애니메이션 중복 방지
        if (animateCoroutine != null && current == displayAmmo)
            return;

        // 기존 애니메이션이 있다면 중지
        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);

        // 1발씩 애니메이션으로 숫자 변화
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
                current--;  // 탄약 감소 시 애니메이션도 부드럽게 적용 가능

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
    /// 외부에서 장전 속도를 조절할 수 있는 함수 (옵션)
    /// </summary>
    public void SetReloadAnimationSpeed(float secondsPerAmmo)
    {
        reloadAnimationSpeed = secondsPerAmmo;
    }
}
