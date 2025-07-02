using UnityEngine;

public class ChainProjectile : MonoBehaviour
{
    public float speed = 20f;
    public LayerMask grappleLayer;

    private Vector2 moveDir;
    private bool hasHit = false;
    private System.Action<Vector2> onHitCallback;

    public void Init(Vector2 direction, System.Action<Vector2> onHit)
    {
        moveDir = direction.normalized;
        onHitCallback = onHit;
    }

    void Update()
    {
        if (!hasHit)
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (((1 << other.gameObject.layer) & grappleLayer) != 0)
        {
            hasHit = true;
            onHitCallback?.Invoke(transform.position);
            Destroy(gameObject);
        }
    }
}
