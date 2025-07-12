using UnityEngine;
using System.Collections;

public class GhostSpawner : MonoBehaviour
{
    public GhostTrailPool trailPool;
    public SpriteRenderer sourceRenderer;
    public float spawnInterval = 0.05f;
    public float fadeDuration = 0.3f;

    void Awake()
    {
        if (sourceRenderer == null)
            sourceRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void StartGhostTrail() => StartCoroutine(SpawnTrail());
    public void StopGhostTrail() => StopAllCoroutines();

    IEnumerator SpawnTrail()
    {
        while (true)
        {
            if (sourceRenderer == null || trailPool == null) yield break;

            GameObject ghost = trailPool.Get();
            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;

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

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
