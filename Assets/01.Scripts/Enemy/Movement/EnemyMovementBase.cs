using UnityEngine;

public abstract class EnemyMovementBase : ScriptableObject
{
    public abstract void Move(EnemyController controller);
}
