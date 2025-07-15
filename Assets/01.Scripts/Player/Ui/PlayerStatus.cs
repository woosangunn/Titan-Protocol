using UnityEngine;
using System;

/// <summary>
/// 플레이어 상태 관리 컴포넌트
/// - 체력 및 탄약 수치와 변경 이벤트를 관리
/// - 외부에서 체력, 탄약 변동 시 UI 등에게 알림 발생
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("Health")]
    public int MaxHealth = 100;          // 최대 체력 값
    public int CurrentHealth = 100;      // 현재 체력 값

    // 체력 변경 시 호출되는 이벤트 (현재 체력, 최대 체력 전달)
    public event Action<int, int> OnHealthChanged;

    [Header("Ammo")]
    public int MaxAmmo = 30;              // 최대 탄약 수
    public int CurrentAmmo = 30;          // 현재 탄약 수

    // 탄약 변경 시 호출되는 이벤트 (현재 탄약, 최대 탄약 전달)
    public event Action<int, int> OnAmmoChanged;

    void Awake()
    {
        // 초기화 시점에 현재 상태를 UI 등에 즉시 알림 (동기화 목적)
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }


    /// <summary>
    /// 체력 감소 처리 메서드 예시
    /// - 데미지를 받아 현재 체력에서 감소
    /// - 최소 0, 최대 MaxHealth 범위로 제한
    /// - 변경 후 이벤트 호출하여 UI 갱신 트리거
    /// </summary>
    /// <param name="damage">입력받은 데미지 값</param>
    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        Debug.Log($"[Player] 피해 {amount} → 현재 체력: {CurrentHealth}");
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.Log("[Player] 사망 처리 로직 실행");
            // TODO: 사망 애니메이션, FSM, 리스폰 등
        }
    }

/// <summary>
/// 탄약 1발 사용 처리
/// - 탄약이 0 이하일 경우 사용 불가
/// - 사용 후 이벤트 호출하여 UI 갱신
/// </summary>
public void UseAmmo()
    {
        if (CurrentAmmo <= 0) return;

        CurrentAmmo--;
        Debug.Log($"[PlayerStatus] UseAmmo → {CurrentAmmo}/{MaxAmmo}");

        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    /// <summary>
    /// 탄약을 최대치로 리로드
    /// - CurrentAmmo를 MaxAmmo로 설정
    /// - 이벤트 호출하여 UI 갱신
    /// </summary>
    public void Reload()
    {
        CurrentAmmo = MaxAmmo;
        Debug.Log("[PlayerStatus] Reload 완료");

        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }
}
