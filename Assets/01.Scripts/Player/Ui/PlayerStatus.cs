using UnityEngine;
using System;

/// <summary>
/// �÷��̾� ���� ���� ������Ʈ
/// - ü�� �� ź�� ��ġ�� ���� �̺�Ʈ�� ����
/// - �ܺο��� ü��, ź�� ���� �� UI ��� �˸� �߻�
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("Health")]
    public int MaxHealth = 100;          // �ִ� ü�� ��
    public int CurrentHealth = 100;      // ���� ü�� ��

    // ü�� ���� �� ȣ��Ǵ� �̺�Ʈ (���� ü��, �ִ� ü�� ����)
    public event Action<int, int> OnHealthChanged;

    [Header("Ammo")]
    public int MaxAmmo = 30;              // �ִ� ź�� ��
    public int CurrentAmmo = 30;          // ���� ź�� ��

    // ź�� ���� �� ȣ��Ǵ� �̺�Ʈ (���� ź��, �ִ� ź�� ����)
    public event Action<int, int> OnAmmoChanged;

    void Awake()
    {
        // �ʱ�ȭ ������ ���� ���¸� UI � ��� �˸� (����ȭ ����)
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    /// <summary>
    /// ü�� ���� ó�� �޼��� ����
    /// - �������� �޾� ���� ü�¿��� ����
    /// - �ּ� 0, �ִ� MaxHealth ������ ����
    /// - ���� �� �̺�Ʈ ȣ���Ͽ� UI ���� Ʈ����
    /// </summary>
    /// <param name="damage">�Է¹��� ������ ��</param>
    public void TakeDamage(int damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        Debug.Log($"[PlayerStatus] TakeDamage �� {CurrentHealth}/{MaxHealth}");

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    /// <summary>
    /// ź�� 1�� ��� ó��
    /// - ź���� 0 ������ ��� ��� �Ұ�
    /// - ��� �� �̺�Ʈ ȣ���Ͽ� UI ����
    /// </summary>
    public void UseAmmo()
    {
        if (CurrentAmmo <= 0) return;

        CurrentAmmo--;
        Debug.Log($"[PlayerStatus] UseAmmo �� {CurrentAmmo}/{MaxAmmo}");

        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    /// <summary>
    /// ź���� �ִ�ġ�� ���ε�
    /// - CurrentAmmo�� MaxAmmo�� ����
    /// - �̺�Ʈ ȣ���Ͽ� UI ����
    /// </summary>
    public void Reload()
    {
        CurrentAmmo = MaxAmmo;
        Debug.Log("[PlayerStatus] Reload �Ϸ�");

        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }
}
