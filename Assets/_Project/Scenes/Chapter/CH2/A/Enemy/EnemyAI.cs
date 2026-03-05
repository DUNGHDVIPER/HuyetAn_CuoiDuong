using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float attackRange = 1.3f;

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public int damage = 10;

    [Header("Attack Hitbox")]
    public Transform attackPoint;
    public float attackRadius = 0.6f;
    public LayerMask playerLayer;

    Transform player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    float attackTimer;
    bool isAttacking;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        sr.flipX = dir < 0;
        attackTimer -= Time.deltaTime;

        // ===== TRONG TẦM ĐÁNH =====
        if (distance <= attackRange)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isMoving", false);

            if (!isAttacking && attackTimer <= 0f)
            {
                StartAttack();
                attackTimer = attackCooldown;
            }
            return;
        }

        // ===== ĐUỔI THEO =====
        anim.SetBool("isMoving", true);
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);
    }

    void StartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        anim.SetTrigger("attack");

        // timing theo animation
        Invoke(nameof(DoDamage), 0.25f);
        Invoke(nameof(EndAttack), 0.6f);
    }

    void DoDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (hit != null)
        {
            PlayerHealthCH2 hp = hit.GetComponent<PlayerHealthCH2>();
            if (hp != null)
                hp.TakeDamage(damage);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    // ⚠️ HÀM GIẢ – DẬP LỖI ANIMATION EVENT CŨ
    public void DealDamage() { }
    public void DoAttack() { }
}