using UnityEngine;
namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth = 100;
        private int currentHealth;
        [SerializeField] private HealthBarPlayer healthBar;
        private bool isDead = false;
        void Start()
        {
            currentHealth = maxHealth;
            if (healthBar != null) healthBar.SetHealth(currentHealth, maxHealth);
        }
        public void TakeDamage(int damage)
        {
            if (isDead) return; 
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            Debug.Log("Player HP: " + currentHealth);
            if (healthBar != null) healthBar.SetHealth(currentHealth, maxHealth);
            if (currentHealth <= 0) { Die(); }
        }
        void Die()
        {
            if (isDead) return;   // ✅ double safety
            isDead = true;
            Debug.Log("Player chết!");

            Animator anim = GetComponent<Animator>();

            // reset mấy cái có thể block
            anim.SetBool("IsJump", false);
            anim.SetBool("Attack", false);
            anim.SetFloat("Speed", 0);

            anim.SetTrigger("Die");

            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerCombat>().enabled = false;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;

            Destroy(gameObject, 2f);
        }
    }
}