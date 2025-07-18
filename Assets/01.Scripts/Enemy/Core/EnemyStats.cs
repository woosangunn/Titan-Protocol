using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    public int MaxHealth = 100;
    public float MoveSpeed = 2f;
    public int Damage = 10;
    public float AttackCooldown = 1f;
    public float AttackRange = 1.5f;

    [HideInInspector] public int CurrentHealth;

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }
}