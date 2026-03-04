using UnityEngine;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class EnemyAI : MonoBehaviour
    {
        public float moveSpeed = 3f;
        public float detectRange = 7f;
        public float attackRange = 1.5f;

        private Rigidbody2D rb;
        private Animator anim;
        private Transform player;
        public EnemyAttackHitbox attackHitbox;

       

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        void Update()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                DoAttack();
            }
            else if (distance <= detectRange)
            {
                DoChase();
            }
            else
            {
                DoIdle();
            }
        }

        void DoChase()
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("Attack", false);

            float dir = player.position.x - transform.position.x;

            if (dir > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);

            rb.velocity = new Vector2(Mathf.Sign(dir) * moveSpeed, rb.velocity.y);
        }

        void DoAttack()
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            anim.SetBool("isRunning", false);
            anim.SetBool("Attack", true);
        }

        void DoIdle()
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            anim.SetBool("isRunning", false);
            anim.SetBool("Attack", false);
        }

        public void EnableHitbox()
        {
            if (attackHitbox != null)
            {
                attackHitbox.EnableHitbox();
                Debug.Log("Hitbox Enabled");
            }
        }

        public void DisableHitbox()
        {
            if (attackHitbox != null)
            {
                attackHitbox.DisableHitbox();
                Debug.Log("Hitbox Disabled");
            }
        }
    }
}