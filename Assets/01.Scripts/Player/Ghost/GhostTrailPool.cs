using UnityEngine;
using System.Collections.Generic;

public class GhostTrailPool : MonoBehaviour
{
    public GameObject ghostPrefab;
    public int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            ghost.SetActive(false);
            pool.Enqueue(ghost);
        }
    }

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
            GameObject extra = Instantiate(ghostPrefab);
            return extra;
        }
    }

    public void Return(GameObject ghost)
    {
        ghost.SetActive(false);
        pool.Enqueue(ghost);
    }
}
