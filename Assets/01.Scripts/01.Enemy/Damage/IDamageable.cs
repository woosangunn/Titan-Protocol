/// <summary>
/// 피격 가능한 모든 대상이 구현해야 하는 인터페이스
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 데미지를 입는 메서드
    /// </summary>
    /// <param name="amount">입힐 데미지 양</param>
    void TakeDamage(int amount);
}
