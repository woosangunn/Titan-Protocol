using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Movement/Static")]
public class StaticMovement : EnemyMovementBase
{
    public override void Move(EnemyController controller)
    {
        // 이동하지 않음
    }
}
