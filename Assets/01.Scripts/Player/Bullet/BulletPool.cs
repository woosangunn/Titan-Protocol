using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �Ѿ� �������� �̸� ������ �ξ��ٰ� �ʿ��� �� ���� ����,
/// ���� ���� �Ǵ� �浹 �� Ǯ�� ��ȯ�Ͽ� ��Ȱ���ϴ� ������Ʈ Ǯ.
/// �̱���(Instance) �������� ��𼭵� ������ �� �ְ� ��.
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }   // �̱��� ����

    [Header("Pool ����")]
    public GameObject bulletPrefab;      // Ǯ���� �Ѿ� ������
    public int initialPoolSize = 50;     // �ʱ� ���� ����

    private Queue<GameObject> pool = new Queue<GameObject>(); // Ǯ ť

    void Awake()
    {
        // �̱��� �ߺ� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // �ʱ� ��ü ����
        for (int i = 0; i < initialPoolSize; i++)
            pool.Enqueue(CreateBullet());
    }

    /// <summary>
    /// Ǯ���� �Ѿ� �ϳ� ���� ��ȯ.
    /// Ǯ�� ������ ���� ����� �ٷ� ��ȯ.
    /// </summary>
    public GameObject GetBullet(Vector2 pos, Quaternion rot)
    {
        GameObject bullet = (pool.Count > 0) ? pool.Dequeue() : CreateBullet();

        bullet.transform.position = pos;
        bullet.transform.rotation = rot;
        bullet.SetActive(true);            // Ȱ��ȭ
        return bullet;
    }

    /// <summary>
    /// �Ѿ��� Ǯ�� �ǵ��� ����.
    /// Bullet ��ũ��Ʈ���� ȣ��.
    /// </summary>
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        pool.Enqueue(bullet);
    }

    // �Ѿ� ������ ��ü�� �����ϰ� Ǯ�� ���� �غ�
    private GameObject CreateBullet()
    {
        GameObject obj = Instantiate(bulletPrefab);
        obj.SetActive(false);
        return obj;
    }
}
