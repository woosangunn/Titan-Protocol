using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 총알 프리팹을 미리 생성해 두었다가 필요할 때 꺼내 쓰고,
/// 수명 종료 또는 충돌 시 풀에 반환하여 재활용하는 오브젝트 풀.
/// 싱글톤(Instance) 패턴으로 어디서든 접근할 수 있게 함.
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }   // 싱글톤 참조

    [Header("Pool 설정")]
    public GameObject bulletPrefab;      // 풀링할 총알 프리팹
    public int initialPoolSize = 50;     // 초기 생성 개수

    private Queue<GameObject> pool = new Queue<GameObject>(); // 풀 큐

    void Awake()
    {
        // 싱글톤 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 초기 객체 생성
        for (int i = 0; i < initialPoolSize; i++)
            pool.Enqueue(CreateBullet());
    }

    /// <summary>
    /// 풀에서 총알 하나 꺼내 반환.
    /// 풀에 없으면 새로 만들고 바로 반환.
    /// </summary>
    public GameObject GetBullet(Vector2 pos, Quaternion rot)
    {
        GameObject bullet = (pool.Count > 0) ? pool.Dequeue() : CreateBullet();

        bullet.transform.position = pos;
        bullet.transform.rotation = rot;
        bullet.SetActive(true);            // 활성화
        return bullet;
    }

    /// <summary>
    /// 총알을 풀에 되돌려 재사용.
    /// Bullet 스크립트에서 호출.
    /// </summary>
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        pool.Enqueue(bullet);
    }

    // 총알 프리팹 실체를 생성하고 풀에 넣을 준비
    private GameObject CreateBullet()
    {
        GameObject obj = Instantiate(bulletPrefab);
        obj.SetActive(false);
        return obj;
    }
}
