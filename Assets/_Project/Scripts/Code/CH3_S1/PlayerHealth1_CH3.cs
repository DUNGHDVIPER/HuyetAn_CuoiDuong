using UnityEngine;

public class PlayerHealth1_CH3 : MonoBehaviour
{
    public int maxHP = 100;
    public HealthBarUI healthBar;

    public int CurrentHP => hp;

    private int hp;
    private bool isDead = false;

    void Awake()
    {
        hp = maxHP;
        Time.timeScale = 1f;   // đảm bảo game chạy bình thường khi bắt đầu scene
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
        if (isDead) return;   // tránh trừ máu sau khi chết

        hp -= dmg;
        if (hp < 0) hp = 0;

        Debug.Log("Player HP = " + hp);

        if (healthBar != null)
            healthBar.SetHP(hp);

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("PLAYER DEAD");

        Time.timeScale = 0f;   // 🔥 DỪNG GAME
    }
}