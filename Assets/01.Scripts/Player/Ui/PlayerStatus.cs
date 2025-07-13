using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int MaxHealth = 100;         // 최대 체력
    public int CurrentHealth = 100;     // 현재 체력

    public int MaxAmmo = 30;            // 최대 탄약
    public int CurrentAmmo = 30;        // 현재 탄약

    void Awake()
    {
        Debug.Log("PlayerStatus Awake 호출됨");
    }

    // 데미지를 입었을 때 체력 감소
    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        Debug.Log($"TakeDamage 호출: 현재 체력 {CurrentHealth}");
    }

    // 체력 회복
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        Debug.Log($"Heal 호출: 현재 체력 {CurrentHealth}");
    }

    // 탄약 사용
    public void UseAmmo()
    {
        if (CurrentAmmo > 0)
        {
            CurrentAmmo--;
            Debug.Log($"UseAmmo 호출: 현재 탄약 {CurrentAmmo}");
        }
    }

    // 탄약 리로드 시작
    public void Reload()
    {
        CurrentAmmo = MaxAmmo;
        Debug.Log("Reload 호출: 탄약 리로드 완료");
    }
}
