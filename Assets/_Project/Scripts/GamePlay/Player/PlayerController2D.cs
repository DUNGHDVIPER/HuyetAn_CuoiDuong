/*using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveX;
    private bool jumpPressed;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Keybind cố định
        moveX = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = 1f;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;

        // Ground check
        if (groundCheck != null)
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        // Move
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (jumpPressed && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        jumpPressed = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
*/