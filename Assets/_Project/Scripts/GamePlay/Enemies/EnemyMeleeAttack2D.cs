using UnityEngine;

public class EnemyMeleeAttack2D : MonoBehaviour
{
    [Header("Refs")]
    public Transform attackPoint;
    public LayerMask playerLayer;
    public SimpleChaseRB chase;
    public Animator anim;

    [Header("Attack")]
    public float attackRange = 0.9f;
    public int damage = 10;
    public float attackCooldown = 1.2f;

    [Header("VFX (optional)")]
    public GameObject hitVfxPrefab;
    public float hitVfxScale = 0.9f;
    public Vector3 hitVfxOffset = new Vector3(0f, 0.3f, 0f);

    private float nextAttackTime = 0f;
    private EnemyHealth health;

    void Awake()
    {
        if (!chase) chase = GetComponent<SimpleChaseRB>();
        if (!anim) anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        if (!attackPoint)
        {
            var ap = transform.Find("AttackPoint");
            if (ap) attackPoint = ap;
        }
    }

    void Update()
    {
        if (health != null && health.IsDead) return;
        if (Time.time < nextAttackTime) return;
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hit == null) return;

        if (chase) chase.enabled = false;
        if (anim) anim.SetTrigger("Attack");

        nextAttackTime = Time.time + attackCooldown;
        Invoke(nameof(ResumeChase), 0.25f);
    }

    void ResumeChase()
    {
        if (health != null && health.IsDead) return;
        if (chase) chase.enabled = true;
    }

    public void AE_DealDamage()
    {
        if (health != null && health.IsDead) return;
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hit == null) return;

        var ph = hit.GetComponentInParent<PlayerHealth>();
        if (ph != null) ph.TakeDamage(damage);

        // VFX hit
        if (hitVfxPrefab != null)
        {
            Vector2 p = hit.ClosestPoint(attackPoint.position);
            var v = Instantiate(hitVfxPrefab, (Vector3)p + hitVfxOffset, Quaternion.identity);
            v.transform.localScale = Vector3.one * hitVfxScale;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}