using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 총알 프리팹을 미리 생성해 두었다가 필요할 때 꺼내 쓰고,
/// 수명 종료 또는 충돌 시 풀에 반환하여 재활용하는 오브젝트 풀.
/// 싱글톤(Instance) 패턴으로 어디서든 접근할 수 있게 함.
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [Header("풀 타입별 총알 프리팹")]
    public GameObject playerBulletPrefab;
    public GameObject enemyBulletPrefab;

    public int initialSize = 50;

    private Dictionary<BulletType, Queue<Bullet>> pools = new Dictionary<BulletType, Queue<Bullet>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitPool(BulletType.Player, playerBulletPrefab);
        InitPool(BulletType.Enemy, enemyBulletPrefab);
    }

    private void InitPool(BulletType type, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[BulletPoolManager] {type} 프리팹이 비어 있습니다.");
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

    private Bullet CreateBullet(GameObject prefab, BulletType type)
    {
        GameObject obj = Instantiate(prefab);
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.bulletType = type;
        obj.SetActive(false);
        return bullet;
    }

    public Bullet GetBullet(BulletType type, Vector2 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(type, out Queue<Bullet> pool) || pool.Count == 0)
        {
            Debug.LogWarning($"[BulletPoolManager] {type} 풀 부족 → 새로 생성");
            Bullet newBullet = CreateBullet(GetPrefab(type), type);
            return ActivateBullet(newBullet, pos, rot);
        }

        Bullet bullet = pool.Dequeue();
        return ActivateBullet(bullet, pos, rot);
    }

    private Bullet ActivateBullet(Bullet bullet, Vector2 pos, Quaternion rot)
    {
        bullet.transform.SetPositionAndRotation(pos, rot);
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        if (!pools.ContainsKey(bullet.bulletType))
            pools[bullet.bulletType] = new Queue<Bullet>();

        pools[bullet.bulletType].Enqueue(bullet);
    }

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
