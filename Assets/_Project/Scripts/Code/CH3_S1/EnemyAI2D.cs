using UnityEngine;
using System.Linq;

public class EnemyAI2D : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Move")]
    public float moveSpeed = 2.5f;
    public float chaseRange = 50f;
    public float stopRange = 2.0f;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 1.2f;
    public LayerMask playerLayer;
    public int damage = 10;
    public float attackCooldown = 1.0f;

    private Rigidbody2D rb;
    private Animator anim;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;
    private int facing = 1;
    private bool hasSpeed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (anim != null)
            hasSpeed = anim.parameters.Any(p => p.name == "Speed");
    }

    void Update()
    {
        // tự tìm Player nếu chưa gán
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null) return;

        // chỉ tính khoảng cách theo trục X
        float distX = Mathf.Abs(player.position.x - transform.position.x);

        // ngoài vùng đuổi -> đứng yên
        if (distX > chaseRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (hasSpeed) anim.SetFloat("Speed", 0f);
            return;
        }

        // nếu đang đánh thì đứng yên
        if (isAttacking)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (hasSpeed) anim.SetFloat("Speed", 0f);
            return;
        }

        // đủ gần để đánh
        if (distX <= stopRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (hasSpeed) anim.SetFloat("Speed", 0f);

            if (Time.time >= nextAttackTime)
            {
                Debug.Log("Enemy -> DoAttack");
                isAttacking = true;
                nextAttackTime = Time.time + attackCooldown;

                anim.ResetTrigger("DoAttack");
                anim.SetTrigger("DoAttack");
            }
            return;
        }

        // chưa đủ gần -> chạy tới player
        float dir = player.position.x - transform.position.x;
        facing = dir >= 0 ? 1 : -1;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * facing;
        transform.localScale = s;

        rb.velocity = new Vector2(facing * moveSpeed, rb.velocity.y);
        if (hasSpeed) anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    // Event đặt ở frame chém trúng
    public void AE_Attack_Hit()
    {
        Debug.Log("AE_Attack_Hit CALLED");

        if (attackPoint == null)
        {
            Debug.LogWarning("attackPoint NULL");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        Debug.Log("Enemy hits count = " + hits.Length);

        foreach (var hit in hits)
        {
            Debug.Log("Hit object = " + hit.name);

            PlayerHealth1 hp = hit.GetComponentInParent<PlayerHealth1>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                Debug.Log("Enemy dealt damage to Player");
            }
        }
    }

    // Event đặt ở cuối clip attacking
    public void AE_Attack_End()
    {
        Debug.Log("AE_Attack_End CALLED");
        isAttacking = false;
        anim.ResetTrigger("DoAttack");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}