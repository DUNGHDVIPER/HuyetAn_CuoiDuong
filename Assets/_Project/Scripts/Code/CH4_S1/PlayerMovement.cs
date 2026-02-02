using UnityEngine;

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
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(move));

        // Flip
        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("IsJump", true);
        }

        // Attack
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
            anim.SetBool("IsJump", false);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit: " + enemy.name);
        }
    }
}
