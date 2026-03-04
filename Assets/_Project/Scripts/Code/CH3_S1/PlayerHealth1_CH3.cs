using UnityEngine;

public class PlayerHealth1 : MonoBehaviour
{
    public int maxHP = 100;
    public HealthBarUI healthBar;   // kéo PlayerHPBar vào đây

    private int hp;

    void Awake()
    {
        hp = maxHP;
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
        hp -= dmg;
        if (hp < 0) hp = 0;

        Debug.Log("Player HP = " + hp);

        if (healthBar != null)
            healthBar.SetHP(hp);

        if (hp <= 0)
        {
            Debug.Log("PLAYER DEAD");
        }
    }
}