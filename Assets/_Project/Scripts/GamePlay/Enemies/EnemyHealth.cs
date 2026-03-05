using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    int currentHealth;

    public RectTransform hpFill;

    Animator anim;
    bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        UpdateBar();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateBar();

        if (anim != null)
            anim.SetTrigger("Hurt");

        if (currentHealth == 0)
            Die();
    }

    void UpdateBar()
    {
        if (hpFill == null) return;

        float percent = (float)currentHealth / maxHealth;
        hpFill.localScale = new Vector3(percent, 1f, 1f);
    }

    void Die()
    {
        isDead = true;
        if (anim != null)
            anim.SetTrigger("Die");

        // Optional: destroy enemy
        // Destroy(gameObject, 1.5f);
    }
}