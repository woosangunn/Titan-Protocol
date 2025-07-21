using UnityEngine;

/// <summary>
/// 적 AI의 두뇌 역할을 하는 컴포넌트
/// - EnemyMovementBase와 EnemyAttackBase를 조합하여 행동 결정 및 실행
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    [Tooltip("적 이동 로직 컴포넌트")]
    public EnemyMovementBase movement;

    [Tooltip("적 공격 로직 컴포넌트")]
    public EnemyAttackBase attack;

    /// <summary>
    /// 적 컨트롤러를 받아 이동과 공격을 수행
    /// </summary>
    /// <param name="controller">적 상태 및 정보가 담긴 컨트롤러</param>
    public void Tick(EnemyController controller)
    {
        movement?.Move(controller);
        attack?.Attack(controller);
    }
}
