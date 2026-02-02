using UnityEngine;

/// <summary>
/// Health dùng chung cho mọi Enemy.
/// Nếu isFinalBoss = true và HP <= 0 -> Unlock WinZone.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 10;
    public int currentHP;

    [Header("Final Boss? (Boss của màn BOSS)")]
    public bool isFinalBoss = false;

    [Header("Refs (optional)")]
    public StageRule stageRule;

    private bool isDead = false;

    private void Awake()
    {
        currentHP = maxHP;

        // Ưu tiên kéo tay trong Inspector. Nếu quên thì tự tìm.
        if (stageRule == null)
            stageRule = FindFirstObjectByType<StageRule>();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        if (dmg <= 0) return;

        currentHP -= dmg;
        Debug.Log($"[Health] {gameObject.name} took {dmg} -> {currentHP}/{maxHP}");

        if (currentHP <= 0)
            Die();
    }   

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"[Health] {gameObject.name} died");

        if (isFinalBoss && stageRule != null)
        {
            Debug.Log("[Health] FinalBoss died -> Unlock WinZone!");
            stageRule.UnlockWinZone();
        }

        Destroy(gameObject);
    }
}
