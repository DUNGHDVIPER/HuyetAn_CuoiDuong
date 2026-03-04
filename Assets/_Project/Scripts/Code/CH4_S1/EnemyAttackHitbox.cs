using UnityEngine;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class EnemyAttackHitbox : MonoBehaviour
    {
        public int damage = 10;
        public float attackCooldown = 0.5f;

        private BoxCollider2D col;
        private float lastAttackTime;

        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
            col.enabled = true; // luôn bật để test
        }

        public void EnableHitbox()
        {
            col.enabled = true;
        }

        public void DisableHitbox()
        {
            col.enabled = false;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Debug.Log("Trigger with: " + other.name);

            if (!col.enabled) return;
            if (!other.CompareTag("Player")) return;
            if (Time.time - lastAttackTime < attackCooldown) return;

            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                Debug.Log("Damage Player");
                player.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }
}