/*using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    float x;
    bool isGrounded;
    bool wasGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Input
        x = Input.GetAxisRaw("Horizontal");

        // Ground check
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Landing trigger: đang rơi (không grounded) -> vừa chạm đất
        if (!wasGrounded && isGrounded)
        {
            anim.SetTrigger("DoLand");
        }

        // Flip
        if (x != 0) sr.flipX = x < 0;

        // Animator params
        anim.SetFloat("Speed", Mathf.Abs(x));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("YVel", rb.velocity.y);
        Debug.Log($"Grounded={isGrounded}  YVel={rb.velocity.y}");
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
*/
using System.Collections;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    [Header("Crouch")]
    public KeyCode crouchKey = KeyCode.S;   // hoặc LeftControl
    public float crouchHeightFactor = 0.6f; // collider cao còn 60%

    [Header("Dodge")]
    public KeyCode dodgeKey = KeyCode.LeftShift;
    public float dodgeSpeed = 12f;
    public float dodgeDuration = 0.18f;
    public float dodgeCooldown = 0.35f;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    Collider2D col;

    float x;
    bool isGrounded;
    bool wasGrounded;

    // Crouch
    bool isCrouch;
    Vector2 colSizeDefault;
    Vector2 colOffsetDefault;

    // Dodge
    bool isDodging;
    float lastDodgeTime;
    int facing = 1; // 1 right, -1 left

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Save collider default (BoxCollider2D hoặc CapsuleCollider2D đều có size/offset)
        CacheColliderDefault();
    }

    void CacheColliderDefault()
    {
        if (col is BoxCollider2D b)
        {
            colSizeDefault = b.size;
            colOffsetDefault = b.offset;
        }
        else if (col is CapsuleCollider2D c)
        {
            colSizeDefault = c.size;
            colOffsetDefault = c.offset;
        }
    }

    void ApplyCrouchCollider(bool crouch)
    {
        if (col is BoxCollider2D b)
        {
            if (crouch)
            {
                b.size = new Vector2(colSizeDefault.x, colSizeDefault.y * crouchHeightFactor);
                b.offset = new Vector2(colOffsetDefault.x, colOffsetDefault.y - (colSizeDefault.y * (1f - crouchHeightFactor) * 0.5f));
            }
            else
            {
                b.size = colSizeDefault;
                b.offset = colOffsetDefault;
            }
        }
        else if (col is CapsuleCollider2D c)
        {
            if (crouch)
            {
                c.size = new Vector2(colSizeDefault.x, colSizeDefault.y * crouchHeightFactor);
                c.offset = new Vector2(colOffsetDefault.x, colOffsetDefault.y - (colSizeDefault.y * (1f - crouchHeightFactor) * 0.5f));
            }
            else
            {
                c.size = colSizeDefault;
                c.offset = colOffsetDefault;
            }
        }
    }

    void Update()
    {
        // Ground check
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        // Input move (khóa khi dodge)
        x = isDodging ? 0 : Input.GetAxisRaw("Horizontal");

        // Facing + Flip (chỉ lật trái/phải)
        if (x != 0)
        {
            facing = x > 0 ? 1 : -1;
            sr.flipX = facing == -1;
        }

        // Crouch
        bool crouchHold = Input.GetKey(crouchKey);
        isCrouch = crouchHold && isGrounded && !isDodging && Mathf.Abs(x) < 0.1f;
        ApplyCrouchCollider(isCrouch);

        // Jump (chặn khi crouch/dodge)
        if (!isDodging && !isCrouch && Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Landing trigger
        if (!wasGrounded && isGrounded)
        {
            anim.SetTrigger("DoLand");
        }

        // Dodge
        if (Input.GetKeyDown(dodgeKey) && CanDodge())
        {
            StartCoroutine(CoDodge());
        }

        // Animator params (nhớ tạo IsCrouch, IsDodging, DoDodge)
        anim.SetFloat("Speed", Mathf.Abs(x));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("YVel", rb.velocity.y);
        anim.SetBool("IsCrouch", isCrouch);
        anim.SetBool("IsDodging", isDodging);
    }

    void FixedUpdate()
    {
        if (isDodging) return;
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
    }

    bool CanDodge()
    {
        if (!isGrounded) return false;
        if (isDodging) return false;
        if (isCrouch) return false;
        if (Time.time < lastDodgeTime + dodgeCooldown) return false;
        return true;
    }

    IEnumerator CoDodge()
    {
        isDodging = true;
        lastDodgeTime = Time.time;

        anim.SetTrigger("DoDodge");

        float t = 0f;
        while (t < dodgeDuration)
        {
            rb.velocity = new Vector2(facing * dodgeSpeed, 0f);
            t += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
