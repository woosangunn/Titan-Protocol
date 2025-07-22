using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 총알 프리팹을 미리 생성해 두었다가 필요할 때 꺼내 쓰고,
/// 수명 종료 또는 충돌 시 풀에 반환하여 재활용하는 오브젝트 풀.
/// 싱글톤(Instance) 패턴으로 어디서든 접근할 수 있게 함.
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }  // 싱글톤 인스턴스

    [Header("풀 타입별 총알 프리팹")]
    public GameObject playerBulletPrefab;  // 플레이어용 총알 프리팹
    public GameObject enemyBulletPrefab;   // 적용 총알 프리팹

    public int initialSize = 50;            // 초기 풀 크기

    // 총알 타입별로 관리하는 큐(풀)
    private Dictionary<BulletType, Queue<Bullet>> pools = new Dictionary<BulletType, Queue<Bullet>>();

    private void Awake()
    {
        // 싱글톤 패턴: 이미 인스턴스가 존재하면 자신은 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 각 타입별로 풀 초기화
        InitPool(BulletType.Player, playerBulletPrefab);
        InitPool(BulletType.Enemy, enemyBulletPrefab);
    }

    // 특정 타입의 총알 풀 생성 및 초기화
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

    // 총알 프리팹을 인스턴스화하고 타입 지정, 비활성화 상태로 반환
    private Bullet CreateBullet(GameObject prefab, BulletType type)
    {
        GameObject obj = Instantiate(prefab);
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.bulletType = type;
        obj.SetActive(false);
        return bullet;
    }

    /// <summary>
    /// 총알 풀에서 꺼내서 위치와 회전을 세팅하고 활성화하여 반환
    /// </summary>
    public Bullet GetBullet(BulletType type, Vector2 pos, Quaternion rot)
    {
        // 해당 타입의 풀이 없거나 비었으면 새로 생성
        if (!pools.TryGetValue(type, out Queue<Bullet> pool) || pool.Count == 0)
        {
            Debug.LogWarning($"[BulletPoolManager] {type} 풀 부족 → 새로 생성");
            Bullet newBullet = CreateBullet(GetPrefab(type), type);
            return ActivateBullet(newBullet, pos, rot);
        }

        Bullet bullet = pool.Dequeue();
        return ActivateBullet(bullet, pos, rot);
    }

    // 총알 위치/회전 설정 후 활성화
    private Bullet ActivateBullet(Bullet bullet, Vector2 pos, Quaternion rot)
    {
        bullet.transform.SetPositionAndRotation(pos, rot);
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    /// <summary>
    /// 총알을 다시 풀에 반환 (비활성화 후 저장)
    /// </summary>
    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        if (!pools.ContainsKey(bullet.bulletType))
            pools[bullet.bulletType] = new Queue<Bullet>();

        pools[bullet.bulletType].Enqueue(bullet);
    }

    // 타입에 따른 총알 프리팹 반환 함수
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
