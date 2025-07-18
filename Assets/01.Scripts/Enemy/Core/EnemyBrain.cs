using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    public EnemyMovementBase movement;
    public EnemyAttackBase attack;

    public void Tick(EnemyController controller)
    {
        movement?.Move(controller);
        attack?.Attack(controller);
    }
}