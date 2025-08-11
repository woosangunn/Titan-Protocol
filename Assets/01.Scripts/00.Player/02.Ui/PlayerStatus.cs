using UnityEngine;
using System;
using System.Collections;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    public int MaxHealth = 100;
    public int CurrentHealth = 100;

    public event Action<int, int> OnHealthChanged;

    public int MaxAmmo = 30;
    public int CurrentAmmo = 30;

    public event Action<int, int> OnAmmoChanged;

    public bool IsReloading { get; private set; } = false;

    void Awake()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

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

    public void UseAmmo()
    {
        if (CurrentAmmo <= 0 || IsReloading)
            return;

        CurrentAmmo--;
        Debug.Log($"[PlayerStatus] UseAmmo → {CurrentAmmo}/{MaxAmmo}");
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    public void StartReload(float reloadDuration)
    {
        if (IsReloading || CurrentAmmo == MaxAmmo)
            return;

        IsReloading = true;
        Debug.Log("[PlayerStatus] 장전 시작");
        StartCoroutine(ReloadCoroutine(reloadDuration));
    }

    private IEnumerator ReloadCoroutine(float duration)
    {
        if (CurrentAmmo >= MaxAmmo)
        {
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
