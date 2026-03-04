using UnityEngine;
using System.Collections;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        public Animator anim;
        public Rigidbody2D rb;
        public GameObject hitbox;
        public GameObject fireEffect;
        public Transform effectPoint;

        [Header("Damage")]
        public float normalDamage = 4;
        public float heavyDamage = 50;
        public float dashDamage = 40;

        [Header("Force")]
        public float dashForce = 8f;

        [Header("Timing")]
        public float normalAttackTime = 0.4f;
        public float heavyAttackTime = 0.6f;
        public float dashAttackTime = 0.5f;

        private float currentDamage;
        private bool isAttacking;
        public bool IsAttacking => isAttacking;

        void Start()
        {
            if (hitbox != null)
                hitbox.SetActive(false);
        }

        void Update()
        {
            Debug.Log("isAttacking: " + isAttacking);
            if (isAttacking) return;

            if (Input.GetMouseButtonDown(0))
                StartCoroutine(NormalAttack());

            if (Input.GetKeyDown(KeyCode.Q))
                StartCoroutine(HeavyAttack());

            if (Input.GetKeyDown(KeyCode.R))
                StartCoroutine(DashAttack());
        }

        IEnumerator NormalAttack()
        {
            isAttacking = true;
            currentDamage = normalDamage;

            anim.SetTrigger("Attack");

            hitbox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            hitbox.SetActive(false);

            yield return new WaitForSeconds(normalAttackTime - 0.2f);

            isAttacking = false;
        }

        IEnumerator HeavyAttack()
        {
            isAttacking = true;
            currentDamage = heavyDamage;

            anim.SetTrigger("Heavy");

            SpawnFire();   // hiệu ứng lửa

            hitbox.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            hitbox.SetActive(false);

            yield return new WaitForSeconds(heavyAttackTime - 0.3f);

            isAttacking = false;
        }

        IEnumerator DashAttack()
        {
            isAttacking = true;
            currentDamage = dashDamage;

            anim.SetTrigger("Attack");

            float dir = transform.localScale.x;
            rb.AddForce(new Vector2(dir * dashForce, 0f), ForceMode2D.Impulse);

            hitbox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            hitbox.SetActive(false);

            yield return new WaitForSeconds(dashAttackTime - 0.2f);

            isAttacking = false;
        }

        void SpawnFire()
        {

            if (fireEffect == null || effectPoint == null)
            {
                Debug.LogError("FireEffect or EffectPoint missing!");
                return;
            }

            GameObject fire = Instantiate(
                fireEffect,
                effectPoint.position,
                Quaternion.identity
            );

            float dir = transform.localScale.x;

            FireProjectile proj = fire.GetComponent<FireProjectile>();
            if (proj != null)
                proj.SetDirection(dir);
            Debug.Log("Spawn: " + fireEffect.name);
        }

        public float GetDamage()
        {
            return currentDamage;
        }
    }
}