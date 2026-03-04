using UnityEngine;
using System.Collections;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class EnemyController : MonoBehaviour
    {
        [Header("References")]
        private Animator animator;
        private Rigidbody2D rb;
        private Transform player;

        [Header("Movement")]
        public float moveSpeed = 2f;
        public float chaseSpeed = 5f;
        public float detectRange = 6f;
        public float attackRange = 1.5f;

        [Header("Combat")]
        public int damage = 10;
        public float knockbackForce = 5f;

        [Header("Health")]
        public float maxHealth = 100f;
        private float currentHealth;
        [SerializeField] private HealthBarEnemy healthBar;

        private bool isDead = false;
        private bool isHit = false;

        void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            currentHealth = maxHealth;

            if (healthBar != null)
                healthBar.SetHealth(currentHealth, maxHealth);
        }

        void Update()
        {
            if (isDead || player == null || isHit) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= detectRange)
            {
                if (distance > attackRange)
                    ChasePlayer();
                else
                    AttackPlayer();
            }
            else
            {
                Idle();
            }
        }

        // ================= DAMAGE =================

        public void TakeDamage(float damageAmount)
        {
            if (isDead) return;

            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (healthBar != null)
                healthBar.SetHealth(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(HitReaction());
            }
        }

        IEnumerator HitReaction()
        {
            isHit = true;

            animator.SetTrigger("Hit");

            float dir = Mathf.Sign(transform.position.x - player.position.x);

            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * knockbackForce, 2f), ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.25f);

            isHit = false;
        }

        // ================= AI =================

        void ChasePlayer()
        {
            animator.SetBool("isRunning", true);

            float dir = Mathf.Sign(player.position.x - transform.position.x);

            rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
            transform.localScale = new Vector3(dir, 1, 1);
        }

        void AttackPlayer()
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Attack");
        }

        void Idle()
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
        }

        void Die()
        {
            isDead = true;
            rb.velocity = Vector2.zero;

            animator.SetTrigger("Die");

            Destroy(gameObject, 1f);
        }
    }
}