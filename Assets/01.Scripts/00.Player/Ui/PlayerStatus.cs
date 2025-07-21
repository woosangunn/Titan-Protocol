using UnityEngine;
using System;
using System.Collections; // 코루틴 사용

/// <summary>
/// 플레이어 상태 관리 컴포넌트
/// - 체력 및 탄약 수치와 변경 이벤트를 관리
/// </summary>
public class PlayerStatus : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [Tooltip("플레이어 최대 체력")]
    public int MaxHealth = 100;

    [Tooltip("플레이어 현재 체력")]
    public int CurrentHealth = 100;

    /// <summary>체력 변경 시 이벤트 (현재 체력, 최대 체력)</summary>
    public event Action<int, int> OnHealthChanged;

    [Header("Ammo")]
    [Tooltip("플레이어 최대 탄약")]
    public int MaxAmmo = 30;

    [Tooltip("플레이어 현재 탄약")]
    public int CurrentAmmo = 30;

    /// <summary>탄약 변경 시 이벤트 (현재 탄약, 최대 탄약)</summary>
    public event Action<int, int> OnAmmoChanged;

    [Tooltip("장전 중인지 여부 (읽기 전용)")]
    public bool IsReloading { get; private set; } = false;

    void Awake()
    {
        // 시작 시 초기 체력/탄약 상태 이벤트 발행
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    /// <summary>
    /// 플레이어가 피해를 입었을 때 호출
    /// </summary>
    /// <param name="amount">입을 피해량</param>
    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        Debug.Log($"[Player] 피해 {amount} → 현재 체력: {CurrentHealth}");
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.Log("[Player] 사망 처리 로직 실행");
            // TODO: 사망 처리 관련 추가 구현 필요
        }
    }

    /// <summary>
    /// 탄약을 1개 소비
    /// </summary>
    public void UseAmmo()
    {
        if (CurrentAmmo <= 0 || IsReloading)
            return;

        CurrentAmmo--;
        Debug.Log($"[PlayerStatus] UseAmmo → {CurrentAmmo}/{MaxAmmo}");
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    /// <summary>
    /// 장전을 시작 (코루틴으로 탄약이 1씩 차오름)
    /// </summary>
    /// <param name="reloadDuration">장전 완료까지 걸리는 시간(초)</param>
    public void StartReload(float reloadDuration)
    {
        if (IsReloading || CurrentAmmo == MaxAmmo)
            return;

        IsReloading = true;
        Debug.Log("[PlayerStatus] 장전 시작");
        StartCoroutine(ReloadCoroutine(reloadDuration));
    }

    /// <summary>
    /// 코루틴: 일정 시간에 걸쳐 탄약 1씩 채움
    /// </summary>
    /// <param name="duration">총 장전 시간</param>
    /// <returns></returns>
    private IEnumerator ReloadCoroutine(float duration)
    {
        if (CurrentAmmo >= MaxAmmo)
        {
            // 이미 가득 차있으면 바로 종료
            IsReloading = false;
            yield break;
        }

        int ammoToReload = MaxAmmo - CurrentAmmo;
        float interval = duration / ammoToReload;

        while (CurrentAmmo < MaxAmmo)
        {
            CurrentAmmo++;
            OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);

            yield return new WaitForSeconds(interval);
        }

        IsReloading = false;
        Debug.Log("[PlayerStatus] 장전 완료");
    }
}
