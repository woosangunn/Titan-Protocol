using UnityEngine;

/// <summary>
/// 적의 이동 동작을 정의하는 추상 기본 클래스
/// - EnemyController가 이동 처리를 위임하는 컴포넌트
/// - ScriptableObject로 구현하여 다양한 이동 패턴을 쉽게 생성 가능
/// </summary>
public abstract class EnemyMovementBase : ScriptableObject
{
    /// <summary>
    /// 매 프레임 EnemyController의 이동을 처리하는 추상 메서드
    /// </summary>
    /// <param name="controller">이동을 수행할 EnemyController 인스턴스</param>
    public abstract void Move(EnemyController controller);
}
