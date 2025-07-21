using UnityEngine;

/// <summary>
/// 적의 스탯 정보 ScriptableObject
/// - 최대 체력, 이동 속도, 공격력, 공격 쿨타임, 공격 거리 포함
/// - 런타임용 현재 체력 관리
/// - 복제(클론) 기능 제공
/// </summary>
[CreateAssetMenu(menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    [Tooltip("최대 체력")]
    public int MaxHealth = 100;
    [Tooltip("이동 속도")]
    public float MoveSpeed = 2f;
    [Tooltip("공격력")]
    public int Damage = 10;
    [Tooltip("공격 쿨타임 (초)")]
    public float AttackCooldown = 1f;
    [Tooltip("공격 사거리")]
    public float AttackRange = 1.5f;

    [HideInInspector]
    [Tooltip("현재 체력 (런타임에만 사용)")]
    public int CurrentHealth;

    /// <summary>
    /// 런타임 시작 시 초기화용 메서드
    /// - 현재 체력을 최대 체력으로 세팅
    /// </summary>
    public void InitializeRuntimeValues()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// 이 ScriptableObject를 복제하여 새로운 인스턴스 생성
    /// - 복제본은 런타임에서 독립적으로 사용 가능
    /// </summary>
    /// <returns>복제된 EnemyStats 객체</returns>
    public EnemyStats Clone()
    {
        EnemyStats clone = Instantiate(this);
        clone.InitializeRuntimeValues();
        return clone;
    }
}
