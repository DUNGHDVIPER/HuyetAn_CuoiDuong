using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D_CH3 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("VFX")]
    public Transform vfxSpawn;
    public GameObject attackPrefab;
    public GameObject firePrefab;
    public GameObject lightningPrefab;
    public float vfxLifeTime = 2f;

    [Header("Damage")]
    public LayerMask enemyLayer;
    public int skillDamage = 20;
    public float skillRadius = 1.2f;

    [Header("Action Lock")]
    public float actionTimeout = 1.2f;

    Rigidbody2D rb;
    Animator anim;

    float xInput;
    bool grounded;
    int facing = 1;

    float busyTimer = 0f;
    bool isBusy = false;

    // Animator Params
    const string P_SPEED = "Speed";
    const string P_GROUNDED = "IsGrounded";
    const string P_YVEL = "YVel";
    const string P_ISBUSY = "IsBusy";

    const string T_ATTACK = "DoAttack";
    const string T_SKILLQ = "DoSkillQ";
    const string T_SKILLE = "DoSkillE";
    const string T_LAND = "DoLand";

    HashSet<string> paramNames = new HashSet<string>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        foreach (var p in anim.parameters)
            paramNames.Add(p.name);
    }

    void Update()
    {
        HandleMovementInput();
        HandleJump();
        HandleActions();
        UpdateAnimator();
        UpdateBusyState();
        
            if (Input.GetMouseButtonDown(0))
            {
                anim.Play("Anim_Attack1", 0, 0);
            
        }
    }


    void FixedUpdate()
    {
        if (!isBusy)
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    // ================= MOVEMENT =================

    void HandleMovementInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (xInput != 0)
        {
            facing = xInput > 0 ? 1 : -1;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * facing;
            transform.localScale = s;
        }
    }

    void HandleJump()
    {
        grounded = IsGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && grounded && !isBusy)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    // ================= ACTIONS =================

    void HandleActions()
    {
        if (isBusy) return;

        // ✅ LEFT MOUSE ATTACK
        if (Input.GetMouseButtonDown(0))
        {
            StartAction(T_ATTACK);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartAction(T_SKILLQ);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartAction(T_SKILLE);
        }
    }

    void StartAction(string trigger)
    {
        if (!HasParam(trigger)) return;

        anim.SetBool(P_ISBUSY, true);
        anim.SetTrigger(trigger);

        isBusy = true;
        busyTimer = Time.time + actionTimeout;
    }

    void UpdateBusyState()
    {
        if (isBusy && Time.time > busyTimer)
        {
            EndBusy();
        }
    }

    public void AnimEvent_EndBusy()
    {
        EndBusy();
    }

    void EndBusy()
    {
        isBusy = false;
        anim.SetBool(P_ISBUSY, false);
    }

    // ================= DAMAGE + VFX =================

    public void AE_Attack_Hit()
    {
        SpawnVFX(attackPrefab);
        DealDamage();
    }

    public void AE_SkillQ_Hit()
    {
        SpawnVFX(firePrefab);
        DealDamage();
    }

    public void AE_SkillE_Hit()
    {
        SpawnVFX(lightningPrefab);
        DealDamage();
    }

    void SpawnVFX(GameObject prefab)
    {
        if (prefab == null || vfxSpawn == null) return;

        GameObject go = Instantiate(prefab, vfxSpawn.position, Quaternion.identity);

        Vector3 s = go.transform.localScale;
        s.x = Mathf.Abs(s.x) * facing;
        go.transform.localScale = s;

        Destroy(go, vfxLifeTime);
    }

    void DealDamage()
    {
        if (vfxSpawn == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            vfxSpawn.position,
            skillRadius,
            enemyLayer
        );

        foreach (var h in hits)
        {
            EnemyHealth1 hp = h.GetComponentInParent<EnemyHealth1>();
            if (hp != null)
                hp.TakeDamage(skillDamage);
        }
    }

    // ================= ANIMATOR =================

    void UpdateAnimator()
    {
        SetFloatSafe(P_SPEED, Mathf.Abs(xInput));
        SetBoolSafe(P_GROUNDED, grounded);
        SetFloatSafe(P_YVEL, rb.velocity.y);
    }

    bool HasParam(string name) => paramNames.Contains(name);

    void SetFloatSafe(string name, float v)
    {
        if (HasParam(name)) anim.SetFloat(name, v);
    }

    void SetBoolSafe(string name, bool v)
    {
        if (HasParam(name)) anim.SetBool(name, v);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        if (vfxSpawn != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(vfxSpawn.position, skillRadius);
        }
    }
}