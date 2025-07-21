using UnityEngine;
using System.Collections.Generic;

public class GhostTrailPool : MonoBehaviour
{
    [Tooltip("���� �̹��� ������")]
    public GameObject ghostPrefab;    // ���� �̹��� ������
    [Tooltip("�ʱ� Ǯ ũ��")]
    public int poolSize = 20;         // �ʱ� Ǯ ũ��

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        // �̸� poolSize��ŭ ���� �̹����� �����Ͽ� ��Ȱ��ȭ �� ť�� ����
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            ghost.SetActive(false);
            pool.Enqueue(ghost);
        }
    }

    /// <summary>
    /// Ǯ���� ���� �̹��� ������Ʈ�� ���� Ȱ��ȭ�Ͽ� ��ȯ
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
            // Ǯ�� ������ ������ ���� ���� �� ��ȯ
            GameObject extra = Instantiate(ghostPrefab);
            return extra;
        }
    }

    /// <summary>
    /// ����� ���� ���� �̹����� ��Ȱ��ȭ �� Ǯ�� ��ȯ
    /// </summary>
    public void Return(GameObject ghost)
    {
        ghost.SetActive(false);
        pool.Enqueue(ghost);
    }
}
