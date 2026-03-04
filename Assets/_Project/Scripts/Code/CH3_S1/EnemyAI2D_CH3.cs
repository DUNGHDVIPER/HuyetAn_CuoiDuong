using UnityEngine;
using System.Linq;

public class EnemyAI2D_CH3 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Move")]
    public float moveSpeed = 2.5f;
    public float chaseRange = 10f;
    public float attackRange = 3f;

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 5f;

    [Header("Hitbox")]
    public Transform attackPoint;
    public float attackRadius = 1.5f;
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Animator anim;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;
    private bool hasDealtDamage = false;
    private bool hasSpeedParam = false;
    private int facing = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (anim != null)
            hasSpeedParam = anim.parameters.Any(p => p.name == "Speed");
    }

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > chaseRange)
        {
            StopMove();
            return;
        }

        if (isAttacking)
        {
            StopMove();
            return;
        }

        if (distance <= attackRange)
        {
            StopMove();

            if (Time.time >= nextAttackTime)
            {
                StartAttack();
            }

            return;
        }

        MoveTowardPlayer();
    }

    void MoveTowardPlayer()
    {
        float dir = player.position.x - transform.position.x;
        facing = dir >= 0 ? 1 : -1;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        transform.localScale = scale;

        rb.velocity = new Vector2(facing * moveSpeed, rb.velocity.y);

        if (hasSpeedParam)
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void StopMove()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);

        if (hasSpeedParam)
            anim.SetFloat("Speed", 0f);
    }

    void StartAttack()
    {
        isAttacking = true;
        hasDealtDamage = false;
        nextAttackTime = Time.time + attackCooldown;

        anim.ResetTrigger("DoAttack");
        anim.SetTrigger("DoAttack");

        Debug.Log("Enemy -> DoAttack");
    }

    // ===== Animation Event =====

    public void AE_Attack_Hit()
    {
        if (hasDealtDamage) return;

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (hit != null)
        {
            PlayerHealth1_CH3 hp = hit.GetComponent<PlayerHealth1_CH3>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                hasDealtDamage = true;
                Debug.Log("Enemy dealt damage");
            }
        }
    }

    public void AE_Attack_End()
    {
        isAttacking = false;
        anim.ResetTrigger("DoAttack");
        Debug.Log("Attack finished");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}