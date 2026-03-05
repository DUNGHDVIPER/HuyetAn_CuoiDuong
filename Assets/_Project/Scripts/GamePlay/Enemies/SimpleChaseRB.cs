using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleChaseRB : MonoBehaviour
{
    public float speed = 2.2f;
    public float stopDistance = 0.9f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogError("[SimpleChaseRB] Cannot find Player tag!", this);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float dist = toPlayer.magnitude;

        // Đứng lại khi đủ gần
        if (dist <= stopDistance)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (anim) anim.SetFloat("Speed", 0f);
            return;
        }

        Vector2 dir = toPlayer / dist; // normalized

        // Move
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

        // Gửi Speed cho animator để chuyển Idle/Walk
        if (anim) anim.SetFloat("Speed", speed);

        // Flip mặt theo hướng X
        if (dir.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // Nếu script bị disable (ví dụ lúc enemy attack), set Speed = 0 để Idle
    void OnDisable()
    {
        if (anim) anim.SetFloat("Speed", 0f);
    }
}