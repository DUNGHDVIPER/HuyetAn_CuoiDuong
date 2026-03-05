using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 90;
    public int currentHP { get; private set; }

    public bool IsDead { get; private set; }

    public event Action OnDeath;

    [Header("Anim")]
    public Animator anim;
    public float destroyDelay = 1.2f;

    [Header("UI/FX")]
    public bool showDamagePopup = true;
    public float popupYOffset = 1.0f;

    void Awake()
    {
        currentHP = maxHP;
        if (!anim) anim = GetComponent<Animator>();
    }

    public void TakeDamage(int dmg)
    {
        if (IsDead) return;
        if (dmg <= 0) return;

        currentHP -= dmg;

        // ====== UI: Damage popup ======
        if (showDamagePopup)
        {
            Vector3 pos = transform.position + Vector3.up * popupYOffset;
            DamagePopup.Spawn(dmg, pos, new Color(1f, 0.9f, 0.2f, 1f)); // vàng nhạt
        }

        // Hurt anim
        if (anim) anim.SetTrigger("Hurt");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (anim) anim.SetBool("IsDead", true);

        var chase = GetComponent<SimpleChaseRB>();
        if (chase) chase.enabled = false;

        var atk = GetComponent<EnemyMeleeAttack2D>();
        if (atk) atk.enabled = false;

        OnDeath?.Invoke();
        Destroy(gameObject, destroyDelay);
    }
}