using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Movement/Static")]
public class StaticMovement : EnemyMovementBase
{
    public override void Move(EnemyController controller)
    {
        // No movement
    }
}