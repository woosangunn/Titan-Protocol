using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int MaxHealth = 100;         // �ִ� ü��
    public int CurrentHealth = 100;     // ���� ü��

    public int MaxAmmo = 30;            // �ִ� ź��
    public int CurrentAmmo = 30;        // ���� ź��

    void Awake()
    {
        Debug.Log("PlayerStatus Awake ȣ���");
    }

    // �������� �Ծ��� �� ü�� ����
    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        Debug.Log($"TakeDamage ȣ��: ���� ü�� {CurrentHealth}");
    }

    // ü�� ȸ��
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        Debug.Log($"Heal ȣ��: ���� ü�� {CurrentHealth}");
    }

    // ź�� ���
    public void UseAmmo()
    {
        if (CurrentAmmo > 0)
        {
            CurrentAmmo--;
            Debug.Log($"UseAmmo ȣ��: ���� ź�� {CurrentAmmo}");
        }
    }

    // ź�� ���ε� ����
    public void Reload()
    {
        CurrentAmmo = MaxAmmo;
        Debug.Log("Reload ȣ��: ź�� ���ε� �Ϸ�");
    }
}
