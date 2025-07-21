using UnityEngine;
using System.Collections.Generic;

public class GhostTrailPool : MonoBehaviour
{
    [Tooltip("유령 이미지 프리팹")]
    public GameObject ghostPrefab;    // 유령 이미지 프리팹
    [Tooltip("초기 풀 크기")]
    public int poolSize = 20;         // 초기 풀 크기

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        // 미리 poolSize만큼 유령 이미지를 생성하여 비활성화 후 큐에 저장
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            ghost.SetActive(false);
            pool.Enqueue(ghost);
        }
    }

    /// <summary>
    /// 풀에서 유령 이미지 오브젝트를 꺼내 활성화하여 반환
    /// </summary>
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            var ghost = pool.Dequeue();
            ghost.SetActive(true);
            return ghost;
        }
        else
        {
            // 풀에 여유가 없으면 새로 생성 후 반환
            GameObject extra = Instantiate(ghostPrefab);
            return extra;
        }
    }

    /// <summary>
    /// 사용이 끝난 유령 이미지를 비활성화 후 풀에 반환
    /// </summary>
    public void Return(GameObject ghost)
    {
        ghost.SetActive(false);
        pool.Enqueue(ghost);
    }
}
