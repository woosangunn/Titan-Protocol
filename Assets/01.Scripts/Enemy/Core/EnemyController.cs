using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public EnemyStats stats;
    public EnemyBrain brain;

    public void TakeDamage(int amount)
    {
        stats.CurrentHealth -= amount;
        if (stats.CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        brain?.Tick(this);
    }
}
