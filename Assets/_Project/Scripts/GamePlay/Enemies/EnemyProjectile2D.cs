using UnityEngine;

public class EnemyProjectile2D : MonoBehaviour
{
    [Header("Runtime")]
    public Vector2 direction = Vector2.right;
    public float speed = 10f;
    public int damage = 6;

    [Header("Hit")]
    public LayerMask playerLayer;

    [Header("VFX (optional)")]
    public GameObject hitVfxPrefab;
    public float hitVfxScale = 0.9f;
    public Vector3 hitVfxOffset = new Vector3(0f, 0.2f, 0f);

    public void Init(Vector2 dir, float spd, int dmg, LayerMask layerMask,
        GameObject hitVfx = null, float hitScale = 0.9f)
    {
        direction = dir.sqrMagnitude < 0.0001f ? Vector2.right : dir.normalized;
        speed = spd;
        damage = dmg;
        playerLayer = layerMask;
        hitVfxPrefab = hitVfx;
        hitVfxScale = hitScale;
    }

    void Update()
    {
        if (StageResultUI.IsShowingResult) return;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (StageResultUI.IsShowingResult) return;

        if (playerLayer.value != 0)
        {
            int mask = 1 << other.gameObject.layer;
            if ((playerLayer.value & mask) == 0) return;
        }

        var ph = other.GetComponentInParent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);

            if (hitVfxPrefab != null)
            {
                Vector2 p = other.ClosestPoint(transform.position);
                var v = Instantiate(hitVfxPrefab, (Vector3)p + hitVfxOffset, Quaternion.identity);
                v.transform.localScale = Vector3.one * hitVfxScale;
            }

            Destroy(gameObject);
        }
    }
}