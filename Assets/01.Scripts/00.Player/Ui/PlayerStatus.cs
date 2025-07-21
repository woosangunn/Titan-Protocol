using UnityEngine;
using System;
using System.Collections; // �ڷ�ƾ ���

/// <summary>
/// �÷��̾� ���� ���� ������Ʈ
/// - ü�� �� ź�� ��ġ�� ���� �̺�Ʈ�� ����
/// </summary>
public class PlayerStatus : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [Tooltip("�÷��̾� �ִ� ü��")]
    public int MaxHealth = 100;

    [Tooltip("�÷��̾� ���� ü��")]
    public int CurrentHealth = 100;

    /// <summary>ü�� ���� �� �̺�Ʈ (���� ü��, �ִ� ü��)</summary>
    public event Action<int, int> OnHealthChanged;

    [Header("Ammo")]
    [Tooltip("�÷��̾� �ִ� ź��")]
    public int MaxAmmo = 30;

    [Tooltip("�÷��̾� ���� ź��")]
    public int CurrentAmmo = 30;

    /// <summary>ź�� ���� �� �̺�Ʈ (���� ź��, �ִ� ź��)</summary>
    public event Action<int, int> OnAmmoChanged;

    [Tooltip("���� ������ ���� (�б� ����)")]
    public bool IsReloading { get; private set; } = false;

    void Awake()
    {
        // ���� �� �ʱ� ü��/ź�� ���� �̺�Ʈ ����
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    /// <summary>
    /// �÷��̾ ���ظ� �Ծ��� �� ȣ��
    /// </summary>
    /// <param name="amount">���� ���ط�</param>
    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        Debug.Log($"[Player] ���� {amount} �� ���� ü��: {CurrentHealth}");
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.Log("[Player] ��� ó�� ���� ����");
            // TODO: ��� ó�� ���� �߰� ���� �ʿ�
        }
    }

    /// <summary>
    /// ź���� 1�� �Һ�
    /// </summary>
    public void UseAmmo()
    {
        if (CurrentAmmo <= 0 || IsReloading)
            return;

        CurrentAmmo--;
        Debug.Log($"[PlayerStatus] UseAmmo �� {CurrentAmmo}/{MaxAmmo}");
        OnAmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }

    /// <summary>
    /// ������ ���� (�ڷ�ƾ���� ź���� 1�� ������)
    /// </summary>
    /// <param name="reloadDuration">���� �Ϸ���� �ɸ��� �ð�(��)</param>
    public void StartReload(float reloadDuration)
    {
        if (IsReloading || CurrentAmmo == MaxAmmo)
            return;

        IsReloading = true;
        Debug.Log("[PlayerStatus] ���� ����");
        StartCoroutine(ReloadCoroutine(reloadDuration));
    }

    /// <summary>
    /// �ڷ�ƾ: ���� �ð��� ���� ź�� 1�� ä��
    /// </summary>
    /// <param name="duration">�� ���� �ð�</param>
    /// <returns></returns>
    private IEnumerator ReloadCoroutine(float duration)
    {
        if (CurrentAmmo >= MaxAmmo)
        {
            // �̹� ���� �������� �ٷ� ����
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
        Debug.Log("[PlayerStatus] ���� �Ϸ�");
    }
}
