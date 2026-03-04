using UnityEngine;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class PlayerAttackHitbox : MonoBehaviour
    {
        private bool hasHit;
        private PlayerCombat combat;

        private void Start()
        {
            combat = GetComponentInParent<PlayerCombat>();
        }

        private void OnEnable()
        {
            hasHit = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit) return;

            if (other.CompareTag("Enemy"))
            {
                EnemyController enemy = other.GetComponentInParent<EnemyController>();

                if (enemy != null)
                {
                    float damage = combat.GetDamage();
                    enemy.TakeDamage(damage);
                    hasHit = true;
                }
            }
        }
    }
}