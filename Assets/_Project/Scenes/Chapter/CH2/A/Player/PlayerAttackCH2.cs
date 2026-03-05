using UnityEngine;

public class PlayerAttackCH2 : MonoBehaviour
{
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(1.2f, 1f);
    public LayerMask enemyLayer;
    public int damage = 10;
    public float knockbackForce = 6f;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("PLAYER ATTACK"); // 👈 thêm dòng này

        anim.SetTrigger("Attack");

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0,
            enemyLayer
        );

        Debug.Log("HIT COUNT = " + hits.Length); // 👈 thêm

        foreach (Collider2D hit in hits)
        {
            Debug.Log("HIT: " + hit.name); // 👈 thêm

            EnemyHealthCH2 enemy = hit.GetComponent<EnemyHealthCH2>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}