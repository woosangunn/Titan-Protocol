using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �Ѿ� �������� �̸� ������ �ξ��ٰ� �ʿ��� �� ���� ����,
/// ���� ���� �Ǵ� �浹 �� Ǯ�� ��ȯ�Ͽ� ��Ȱ���ϴ� ������Ʈ Ǯ.
/// �̱���(Instance) �������� ��𼭵� ������ �� �ְ� ��.
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }  // �̱��� �ν��Ͻ�

    [Header("Ǯ Ÿ�Ժ� �Ѿ� ������")]
    public GameObject playerBulletPrefab;  // �÷��̾�� �Ѿ� ������
    public GameObject enemyBulletPrefab;   // ���� �Ѿ� ������

    public int initialSize = 50;            // �ʱ� Ǯ ũ��

    // �Ѿ� Ÿ�Ժ��� �����ϴ� ť(Ǯ)
    private Dictionary<BulletType, Queue<Bullet>> pools = new Dictionary<BulletType, Queue<Bullet>>();

    private void Awake()
    {
        // �̱��� ����: �̹� �ν��Ͻ��� �����ϸ� �ڽ��� �ı�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // �� Ÿ�Ժ��� Ǯ �ʱ�ȭ
        InitPool(BulletType.Player, playerBulletPrefab);
        InitPool(BulletType.Enemy, enemyBulletPrefab);
    }

    // Ư�� Ÿ���� �Ѿ� Ǯ ���� �� �ʱ�ȭ
    private void InitPool(BulletType type, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[BulletPoolManager] {type} �������� ��� �ֽ��ϴ�.");
            return;
        }

        Queue<Bullet> pool = new Queue<Bullet>();
        for (int i = 0; i < initialSize; i++)
        {
            Bullet bullet = CreateBullet(prefab, type);
            pool.Enqueue(bullet);
        }

        pools[type] = pool;
    }

    // �Ѿ� �������� �ν��Ͻ�ȭ�ϰ� Ÿ�� ����, ��Ȱ��ȭ ���·� ��ȯ
    private Bullet CreateBullet(GameObject prefab, BulletType type)
    {
        GameObject obj = Instantiate(prefab);
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.bulletType = type;
        obj.SetActive(false);
        return bullet;
    }

    /// <summary>
    /// �Ѿ� Ǯ���� ������ ��ġ�� ȸ���� �����ϰ� Ȱ��ȭ�Ͽ� ��ȯ
    /// </summary>
    public Bullet GetBullet(BulletType type, Vector2 pos, Quaternion rot)
    {
        // �ش� Ÿ���� Ǯ�� ���ų� ������� ���� ����
        if (!pools.TryGetValue(type, out Queue<Bullet> pool) || pool.Count == 0)
        {
            Debug.LogWarning($"[BulletPoolManager] {type} Ǯ ���� �� ���� ����");
            Bullet newBullet = CreateBullet(GetPrefab(type), type);
            return ActivateBullet(newBullet, pos, rot);
        }

        Bullet bullet = pool.Dequeue();
        return ActivateBullet(bullet, pos, rot);
    }

    // �Ѿ� ��ġ/ȸ�� ���� �� Ȱ��ȭ
    private Bullet ActivateBullet(Bullet bullet, Vector2 pos, Quaternion rot)
    {
        bullet.transform.SetPositionAndRotation(pos, rot);
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    /// <summary>
    /// �Ѿ��� �ٽ� Ǯ�� ��ȯ (��Ȱ��ȭ �� ����)
    /// </summary>
    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        if (!pools.ContainsKey(bullet.bulletType))
            pools[bullet.bulletType] = new Queue<Bullet>();

        pools[bullet.bulletType].Enqueue(bullet);
    }

    // Ÿ�Կ� ���� �Ѿ� ������ ��ȯ �Լ�
    private GameObject GetPrefab(BulletType type)
    {
        return type switch
        {
            BulletType.Player => playerBulletPrefab,
            BulletType.Enemy => enemyBulletPrefab,
            _ => null
        };
    }
}
