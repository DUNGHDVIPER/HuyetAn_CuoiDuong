using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Jump Control")]
    public float jumpCutMultiplier = 0.5f;   // thả sớm -> nhảy thấp
    public float fallMultiplier = 2.5f;      // rơi nhanh cho đã tay

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    float moveInput;
    bool isGrounded;
    bool isAttacking;
    bool isJumpHolding;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ===== MOVE INPUT =====
        moveInput = Input.GetAxisRaw("Horizontal");

        // ===== JUMP HOLD INPUT =====
        isJumpHolding = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);

        // ===== CHECK GROUND =====
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0,
            groundLayer
        );

        // ===== JUMP START =====
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // ===== ATTACK INPUT =====
        if (Input.GetKeyDown(KeyCode.J))
        {
            isAttacking = true;
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            isAttacking = false;
        }

        // ===== ANIMATION =====
        anim.SetBool("isRunning", moveInput != 0);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isAttacking", isAttacking);

        // ===== FLIP =====
        if (moveInput > 0) sr.flipX = false;
        else if (moveInput < 0) sr.flipX = true;
    }

    void FixedUpdate()
    {
        // ===== MOVE =====
        if (!isAttacking)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        // ===== VARIABLE JUMP HEIGHT =====
        if (rb.velocity.y > 0 && !isJumpHolding)
        {
            rb.velocity = new Vector2(
                rb.velocity.x,
                rb.velocity.y * jumpCutMultiplier
            );
        }

        // ===== FAST FALL =====
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up
                * Physics2D.gravity.y
                * (fallMultiplier - 1)
                * Time.fixedDeltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}