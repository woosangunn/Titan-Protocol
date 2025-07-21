using UnityEngine;

/// <summary>
/// 움직이지 않는 정적인 적 이동 패턴
/// - 적이 고정 위치에 머무르도록 함
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Movement/Static")]
public class StaticMovement : EnemyMovementBase
{
    /// <summary>
    /// 이동 없음 (정적)
    /// </summary>
    /// <param name="controller">이동을 수행할 EnemyController 인스턴스</param>
    public override void Move(EnemyController controller)
    {
        // 이동하지 않음
    }
}
