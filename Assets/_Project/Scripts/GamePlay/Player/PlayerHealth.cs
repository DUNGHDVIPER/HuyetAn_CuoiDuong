using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Refs")]
    public StageRule stageRule;
    public PlayerCombat2D combat;
    private bool isDead = false;

    [Header("Block")]
    [Range(0f, 1f)]
    public float blockDamageMultiplier = 0.36f;

    [Header("UI/FX")]
    public float popupYOffset = 1.2f;
    public bool showDamagePopup = true;
    public bool flashOnHit = true;

    private StageHUD _hud;

    private void Awake()
    {
        currentHP = maxHP;

        if (stageRule == null)
            stageRule = FindFirstObjectByType<StageRule>();

        if (combat == null)
            combat = GetComponent<PlayerCombat2D>();

        // cache HUD (null-safe)
        _hud = StageHUD.Main != null ? StageHUD.Main : FindFirstObjectByType<StageHUD>();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        if (dmg <= 0) return;

        bool isBlocking = (combat != null && combat.isBlocking);

        int finalDmg = dmg;

        if (isBlocking)
        {
            finalDmg = Mathf.Max(1, Mathf.RoundToInt(dmg * blockDamageMultiplier));
            Debug.Log($"[PlayerHealth] BLOCK! {dmg} -> {finalDmg}");
        }

        currentHP -= finalDmg;
        Debug.Log($"[PlayerHealth] took {finalDmg} -> {currentHP}/{maxHP}");

        // ====== UI: Damage flash ======
        if (flashOnHit)
        {
            if (_hud == null) _hud = StageHUD.Main != null ? StageHUD.Main : FindFirstObjectByType<StageHUD>();
            if (_hud != null)
            {
                // block thì flash nhẹ hơn
                _hud.FlashDamage(isBlocking ? 0.25f : 0.55f, 0.12f);
            }
        }

        // ====== UI: Damage popup ======
        if (showDamagePopup)
        {
            Vector3 pos = transform.position + Vector3.up * popupYOffset;

            if (isBlocking)
            {
                // chữ BLOCK + số dmg nhỏ
                DamagePopup.SpawnText("BLOCK", pos + Vector3.up * 0.35f, new Color(0.6f, 0.9f, 1f, 1f));
                DamagePopup.Spawn(finalDmg, pos, new Color(1f, 0.6f, 0.6f, 1f));
            }
            else
            {
                DamagePopup.Spawn(finalDmg, pos, Color.red);
            }
        }

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PlayerHealth] Player died");

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