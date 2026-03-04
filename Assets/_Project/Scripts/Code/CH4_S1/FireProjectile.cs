using UnityEngine;
using Assets._Project.Scripts.Code.CH4_S1;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class FireProjectile : MonoBehaviour
    {
        public float speed = 8f;
        public float lifeTime = 2f;
        public float damage = 50f;

        private float direction;

        public void SetDirection(float dir)
        {
            direction = dir;
            Destroy(gameObject, lifeTime);
        }

        void Update()
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyController enemy = other.GetComponentInParent<EnemyController>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}