using UnityEngine;
using System.Collections;

public class GhostSpawner : MonoBehaviour
{
    [Tooltip("������ ���� �̹��� Ǯ")]
    public GhostTrailPool trailPool;           // ������ ���� �̹��� Ǯ
    [Tooltip("���� �̹����� ������ ���� ��������Ʈ ������")]
    public SpriteRenderer sourceRenderer;      // ���� �̹����� ������ ���� ��������Ʈ ������
    [Tooltip("���� �̹��� ���� ����")]
    public float spawnInterval = 0.05f;        // ���� �̹��� ���� ����
    [Tooltip("���� �̹��� ���̵� ���� �ð� (GhostImage���� ���)")]
    public float fadeDuration = 0.3f;          // ���� �̹��� ���̵� ���� �ð� (GhostImage���� ���)

    void Awake()
    {
        // sourceRenderer ������ �� �ڽĿ��� �ڵ� Ž��
        if (sourceRenderer == null)
            sourceRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // ���� Ʈ���� ���� ����
    public void StartGhostTrail() => StartCoroutine(SpawnTrail());

    // ���� Ʈ���� ���� ����
    public void StopGhostTrail() => StopAllCoroutines();

    IEnumerator SpawnTrail()
    {
        while (true)
        {
            // �ʿ� ������Ʈ ���� �� �ڷ�ƾ ����
            if (sourceRenderer == null || trailPool == null) yield break;

            // Ǯ���� ���� �̹��� ������Ʈ ȹ��
            GameObject ghost = trailPool.Get();

            // ���� ��ġ �� ȸ������ ��ġ
            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;

            // GhostImage ������Ʈ �ʱ�ȭ (��������Ʈ, ������, ����, Ǯ ����)
            GhostImage ghostComp = ghost.GetComponent<GhostImage>();
            if (ghostComp != null)
            {
                ghostComp.Init(
                    sourceRenderer.sprite,
                    sourceRenderer.flipX,
                    new Color(1f, 1f, 1f, 0.6f),
                    trailPool
                );
            }

            // ���� �������� ���
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
