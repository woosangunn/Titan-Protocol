using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform gunPivot;
    public Transform gunMuzzle;   // �Ѿ� �߻� ��ġ

    public Transform chainPivot;
    public Transform chainMuzzle; // �罽 �߻� ��ġ

    public GameObject bulletPrefab;
    public GameObject chainPrefab;
    public float bulletSpeed = 10f;
    public float chainSpeed = 10f;

    public void ShootBullet(Vector2 targetPos)
    {
        Vector2 firePos = gunMuzzle.position; // ����!
        Vector2 dir = (targetPos - firePos).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * bulletSpeed;
    }

    public void ShootChain(Vector2 targetPos)
    {
        Vector2 firePos = chainMuzzle.position; // ����!
        Vector2 dir = (targetPos - firePos).normalized;

        GameObject chain = Instantiate(chainPrefab, firePos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        chain.transform.rotation = Quaternion.Euler(0, 0, angle);

        var rb = chain.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * chainSpeed;
    }
}
