using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 플레이어 탄약 UI 표시 컴포넌트
/// - 탄약 변경 시 텍스트 수치 변경 애니메이션 적용
/// - 탄약 수에 따라 글자 색상이 흰색 → 검정색으로 점점 어두워짐
/// </summary>
public class PlayerAmmoUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text ammoText;         // "AMMO: 10 / 30" 형식 텍스트

    private PlayerStatus status;
    private Coroutine animate;
    private int displayAmmo;          // 현재 표시되는 탄약 수
    private int maxAmmo;

    [Header("설정")]
    public float animSpeed = 0.05f;   // 숫자 하나 변화 시 딜레이 (작을수록 빠름)

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

        // 초기 값 즉시 반영 (시작 시 30 / 30처럼)
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
    /// PlayerStatus의 OnAmmoChanged 이벤트로부터 호출됨
    /// </summary>
    public void UpdateAmmoUI(int current, int max)
    {
        maxAmmo = max;

        if (animate != null)
            StopCoroutine(animate);

        if (current == displayAmmo)
        {
            // 숫자 변동 없음
            SetText(displayAmmo, maxAmmo);
            return;
        }

        animate = StartCoroutine(AnimateAmmoChange(current));
    }

    /// <summary>
    /// 탄약 수치가 변경될 때 숫자가 하나씩 빠르게 증감하도록 애니메이션 처리
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
    /// 텍스트 표시 및 색상 보간 처리
    /// </summary>
    private void SetText(int current, int max)
    {
        ammoText.text = $"AMMO: {current} / {max}";

        float ratio = (max > 0) ? (float)current / max : 0f;
        ammoText.color = Color.Lerp(Color.black, Color.white, ratio);
    }
}
