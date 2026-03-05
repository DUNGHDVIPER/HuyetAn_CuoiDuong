using UnityEngine;

public class PlayerHealthCH2 : MonoBehaviour
{
    public int maxHealth = 100;
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
        currentHealth = Mathf.Max(currentHealth, 0);

        anim.SetTrigger("Hurt");
        UpdateBar();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        anim.SetTrigger("Dying");

        GetComponent<PlayerMove>().enabled = false;

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;
    }

    void UpdateBar()
    {
        float percent = (float)currentHealth / maxHealth;
        hpFill.localScale = new Vector3(percent, 1f, 1f);
    }
}