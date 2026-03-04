using UnityEngine;
using System.Linq;

public class EnemyAI2D_CH3 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Move")]
    public float moveSpeed = 2.5f;
    public float chaseRange = 999f;
    public float stopRange = 3.5f;

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 10.0f;

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
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null) return;

        float distX = Mathf.Abs(player.position.x - transform.position.x);

        // Ngoài vùng đuổi
        if (distX > chaseRange)
        {
            StopMove();
            return;
        }

        // Đang đánh thì đứng yên
        if (isAttacking)
        {
            StopMove();
            return;
        }

        // Đủ gần để đánh
        if (distX <= stopRange)
        {
            StopMove();

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

        // Chưa đủ gần → chạy tới player
        float dir = player.position.x - transform.position.x;
        facing = dir >= 0 ? 1 : -1;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * facing;
        transform.localScale = s;

        rb.velocity = new Vector2(facing * moveSpeed, rb.velocity.y);
        if (hasSpeed) anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void StopMove()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        if (hasSpeed) anim.SetFloat("Speed", 0f);
    }

    // 🔥 EVENT ĐẶT Ở FRAME CHÉM TRÚNG
    public void AE_Attack_Hit()
    {
        Debug.Log("AE_Attack_Hit CALLED");

        if (player == null) return;

        float distX = Mathf.Abs(player.position.x - transform.position.x);

        if (distX <= stopRange + 0.5f)
        {
            PlayerHealth1 hp = player.GetComponent<PlayerHealth1>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                Debug.Log("Enemy dealt damage to Player");
            }
        }
    }

    // 🔥 EVENT ĐẶT Ở CUỐI ANIMATION
    public void AE_Attack_End()
    {
        Debug.Log("AE_Attack_End CALLED");
        isAttacking = false;
        anim.ResetTrigger("DoAttack");
    }
}