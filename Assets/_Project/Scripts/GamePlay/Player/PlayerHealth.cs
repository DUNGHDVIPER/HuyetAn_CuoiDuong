using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Refs")]
    public StageRule stageRule;

    private bool isDead = false;

    private void Awake()
    {
        currentHP = maxHP;

        if (stageRule == null)
            stageRule = FindFirstObjectByType<StageRule>();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        if (dmg <= 0) return;

        currentHP -= dmg;
        Debug.Log($"[PlayerHealth] took {dmg} -> {currentHP}/{maxHP}");

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PlayerHealth] Player died");

        // Gọi Lose, KHÔNG destroy player (để còn show UI)
        if (stageRule != null)
            stageRule.TriggerLose("Player HP <= 0");
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        if (amount <= 0) return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }
}
