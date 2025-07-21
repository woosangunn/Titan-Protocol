using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/Attack/Melee Flexible")]
public class MeleeAttack : EnemyAttackBase
{
    [Header("General Settings")]
    [Tooltip("차지 공격 사용 여부")]
    public bool useCharge = false;

    [Tooltip("공격 피해량")]
    public int damage = 10;

    [Header("Charge Settings")]
    [Tooltip("차지 공격 예고 지연 시간 (초)")]
    public float chargeDelay = 0.3f;

    [Tooltip("차지 공격 이동 속도")]
    public float chargeSpeed = 10f;

    [Tooltip("차지 공격 지속 시간 (초)")]
    public float chargeDuration = 0.5f;

    /// <summary>
    /// 공격 수행
    /// - 차지 공격 사용 시 ChargeRoutine 코루틴 실행
    /// - 비사용 시 거리 내에 플레이어 있으면 즉시 피해 적용
    /// </summary>
    /// <param name="controller">공격을 수행하는 적 컨트롤러</param>
    public override void Attack(EnemyController controller)
    {
        // 공격 쿨다운 검사
        if (Time.time < controller.LastAttackTime + controller.Stats.AttackCooldown)
            return;

        if (useCharge)
        {
            controller.StartCoroutine(ChargeRoutine(controller));
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            float distance = Vector2.Distance(controller.transform.position, player.transform.position);
            if (distance <= controller.Stats.AttackRange)
            {
                IDamageable damageable = player.GetComponent<IDamageable>();
                damageable?.TakeDamage(damage);
                controller.LastAttackTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 차지 공격 코루틴
    /// - 예고 후 플레이어 방향으로 이동
    /// - 이동 후 플레이어 충돌 시 피해 적용
    /// </summary>
    /// <param name="controller">적 컨트롤러</param>
    private IEnumerator ChargeRoutine(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        Vector2 dir = (player.transform.position - controller.transform.position).normalized;

        // 텔레그래프(예고) 시각 효과
        ShowTelegraph(controller.transform.position, dir);

        yield return new WaitForSeconds(chargeDelay); // 예고 시간 대기

        float elapsed = 0f;
        while (elapsed < chargeDuration)
        {
            controller.transform.position += (Vector3)(dir * chargeSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 플레이어 충돌 체크 및 피해 적용
        Collider2D hit = Physics2D.OverlapCircle(controller.transform.position, 0.5f, LayerMask.GetMask("Player"));
        if (hit)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();
            dmg?.TakeDamage(damage);
        }

        controller.LastAttackTime = Time.time;
    }

    /// <summary>
    /// 공격 예고를 위한 디버그 레이 표시
    /// - 실제 게임에선 라인 렌더러나 이펙트로 대체 가능
    /// </summary>
    /// <param name="origin">시작 위치</param>
    /// <param name="dir">방향 단위 벡터</param>
    private void ShowTelegraph(Vector2 origin, Vector2 dir)
    {
        Debug.DrawRay(origin, dir * 3f, Color.red, chargeDelay);
    }
}
