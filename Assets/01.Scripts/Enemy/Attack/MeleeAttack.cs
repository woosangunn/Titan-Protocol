using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/Attack/Melee Flexible")]
public class MeleeAttack : EnemyAttackBase
{
    [Header("General Settings")]
    public bool useCharge = false;
    public int damage = 10;

    [Header("Charge Settings")]
    public float chargeDelay = 0.3f;
    public float chargeSpeed = 10f;
    public float chargeDuration = 0.5f;

    public override void Attack(EnemyController controller)
    {
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

    private IEnumerator ChargeRoutine(EnemyController controller)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        Vector2 dir = (player.transform.position - controller.transform.position).normalized;

        // �ڷ��׷��� (�ð��� ��Ʈ)
        ShowTelegraph(controller.transform.position, dir);

        yield return new WaitForSeconds(chargeDelay); // ���� �ð�

        float elapsed = 0f;
        while (elapsed < chargeDuration)
        {
            controller.transform.position += (Vector3)(dir * chargeSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // �浹 �� ����
        Collider2D hit = Physics2D.OverlapCircle(controller.transform.position, 0.5f, LayerMask.GetMask("Player"));
        if (hit)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();
            dmg?.TakeDamage(damage);
        }

        controller.LastAttackTime = Time.time;
    }

    private void ShowTelegraph(Vector2 origin, Vector2 dir)
    {
        // Debug�� ����. ���� ���ӿ����� ���� �������� ����Ʈ�� ��ü ����
        Debug.DrawRay(origin, dir * 3f, Color.red, chargeDelay);
    }
}
