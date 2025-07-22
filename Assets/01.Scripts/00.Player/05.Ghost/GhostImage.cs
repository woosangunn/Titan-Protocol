using UnityEngine;

public class GhostImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private GhostTrailPool pool;

    public float fadeDuration = 0.4f;  // ���̵� �ƿ� ���� �ð�
    private float timer;

    /// <summary>
    /// �ʱ�ȭ �Լ�: ��������Ʈ, �¿� ����, ���� ����, Ǯ ���� ����
    /// </summary>
    public void Init(Sprite sprite, bool flipX, Color startColor, GhostTrailPool trailPool)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        sr.sprite = sprite;          // ��������Ʈ ����
        sr.flipX = flipX;            // �¿� ���� ����
        sr.color = startColor;       // �ʱ� ���� (���� ����)

        pool = trailPool;            // Ǯ ���� ����
        timer = 0f;                  // Ÿ�̸� �ʱ�ȭ
        gameObject.SetActive(true);  // ���� ������Ʈ Ȱ��ȭ
    }

    void Update()
    {
        timer += Time.deltaTime;

        // ���İ��� 0.6 �� 0���� ���� ���� (���̵� �ƿ� ȿ��)
        float alpha = Mathf.Lerp(0.6f, 0f, timer / fadeDuration);

        if (sr != null)
        {
            // ���� ���� �����ϸ鼭 ���İ��� ����
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        }

        // ���̵� �ƿ� �Ϸ� ��
        if (timer >= fadeDuration)
        {
            gameObject.SetActive(false);   // ��Ȱ��ȭ
            pool.Return(gameObject);        // Ǯ�� ��ȯ�Ͽ� ���� ���
        }
    }
}
