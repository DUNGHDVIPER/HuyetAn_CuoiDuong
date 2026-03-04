using System;
using UnityEngine;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 300f;

        public float CurrentHealth { get; private set; }

        public Action<float> OnDamaged;
        public Action OnDeath;

        private bool isDead;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;
            if (damage <= 0f) return;

            CurrentHealth -= damage;
            OnDamaged?.Invoke(damage);

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            CurrentHealth = 0f;

            OnDeath?.Invoke();

            // Nếu muốn chơi animation chết thì đừng Destroy ngay
            Destroy(gameObject);
        }
    }
}