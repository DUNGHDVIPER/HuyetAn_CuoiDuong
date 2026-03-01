using UnityEngine;
using System.Collections;

public class EnemyHealth1 : MonoBehaviour
{
    public int maxHP = 10;
    public float destroyDelay = 0.8f;

    private int hp;
    private bool isDead = false;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    private EnemyAI2D ai;
    private HealthBarUI healthBar;

    public bool IsDead => isDead;

    void Awake()
    {
        hp = maxHP;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ai = GetComponent<EnemyAI2D>();

        // tự tìm thanh máu trong con của quái
        healthBar = GetComponentInChildren<HealthBarUI>(true);
    }

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHP(maxHP);
            healthBar.SetHP(hp);
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        hp -= dmg;
        if (hp < 0) hp = 0;

        Debug.Log($"{name} HP = {hp}");

        if (healthBar != null)
            healthBar.SetHP(hp);

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            if (anim != null)
                anim.SetTrigger("DoHurt");
        }
    }

    void Die()
    {
        isDead = true;

        if (ai != null) ai.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        if (col != null)
            col.enabled = false;

        if (anim != null)
            anim.SetBool("IsDead", true);

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}