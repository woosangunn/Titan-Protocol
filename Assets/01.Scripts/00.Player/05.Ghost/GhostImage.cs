using UnityEngine;

public class GhostImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private GhostTrailPool pool;

    public float fadeDuration = 0.4f;
    private float timer;

    public void Init(Sprite sprite, bool flipX, Color startColor, GhostTrailPool trailPool)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        sr.sprite = sprite;
        sr.flipX = flipX;
        sr.color = startColor;

        pool = trailPool;
        timer = 0f;
        gameObject.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(0.6f, 0f, timer / fadeDuration);

        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        }

        if (timer >= fadeDuration)
        {
            gameObject.SetActive(false);
            pool.Return(gameObject);
        }
    }
}
