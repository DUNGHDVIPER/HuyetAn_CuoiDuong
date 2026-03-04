using Assets._Project.Scripts.Code.CH4_S1;
using UnityEngine;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 8f;

        private Rigidbody2D rb;
        private Animator anim;
        private bool isGrounded;
        private bool facingRight = true;

        public Transform groundCheck;
        public LayerMask groundLayer;

        public Transform attackPoint;
        public float attackRange = 4f;
        public LayerMask enemyLayer;
        public GameObject attackHitbox;
        private PlayerCombat combat;


        void Start()
        {
            combat = GetComponent<PlayerCombat>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            anim.SetBool("IsJump", false);

        }
        void Update()
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                0.2f,
                groundLayer
            );

            float move = Input.GetAxisRaw("Horizontal");

            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
            anim.SetFloat("Speed", Mathf.Abs(move));

            if (move > 0 && !facingRight)
                Flip();
            else if (move < 0 && facingRight)
                Flip();

            // Jump
            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                anim.SetBool("IsJump", true);
            }

            // Reset khi đã rơi xuống lại mặt đất
            if (isGrounded && rb.velocity.y <= 0)
            {
                anim.SetBool("IsJump", false);
            }
        }



            //void FixedUpdate()
            //{
            //    isGrounded = Physics2D.OverlapCircle(
            //        groundCheck.position, 0.2f, groundLayer);

            //    if (isGrounded)
            //        anim.SetBool("IsJump", false);
            //}

            void Flip()
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        //void Attack()
        //{
        //    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
        //        attackPoint.position,
        //        attackRange,
        //        enemyLayer
        //    );

        //    foreach (Collider2D enemy in hitEnemies)
        //    {
        //        EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
        //        if (eh != null)
        //        {
        //            eh.TakeDamage(20);
        //        }
        //    }
        //}

        public void EnableHitbox()
        {
            attackHitbox.SetActive(true);
        }

        public void DisableHitbox()
        {
            attackHitbox.SetActive(false);
        }

    }
}