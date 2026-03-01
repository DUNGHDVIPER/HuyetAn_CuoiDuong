using System.Collections.Generic;
using UnityEngine;

public class PlayerConroller2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("VFX")]
    public Transform vfxSpawn;
    public GameObject firePrefab;
    public GameObject lightningPrefab;
    public GameObject attackPrefab;
    public float vfxLifeTime = 2f;

    [Header("Damage")]
    public LayerMask enemyLayer;       // tick Enemy
    public int skillDamage = 20;
    public float skillRadius = 1.2f;

    [Header("Safety")]
    public float actionTimeout = 1.2f;

    Rigidbody2D rb;
    Animator anim;

    bool grounded, wasGrounded;
    float xInput;
    int facing = 1;

    enum ActionType { None, Attack, SkillQ, SkillE }
    ActionType currentAction = ActionType.None;
    float busyUntil = 0f;

    const string P_SPEED = "Speed";
    const string P_GROUNDED = "IsGrounded";
    const string P_YVEL = "YVel";
    const string P_ISBUSY = "IsBusy";

    const string T_LAND = "DoLand";
    const string T_ATTACK = "DoAttack";
    const string T_SKILLQ = "DoSkillQ";
    const string T_SKILLE = "DoSkillE";

    HashSet<string> paramNames = new HashSet<string>();
    bool warnedMissing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        foreach (var p in anim.parameters)
            paramNames.Add(p.name);
    }

    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        // flip + facing
        if (xInput != 0)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (xInput > 0 ? 1 : -1);
            transform.localScale = s;
        }
        facing = transform.localScale.x >= 0 ? 1 : -1;

        // grounded
        wasGrounded = grounded;
        grounded = IsGrounded();

        SetFloatSafe(P_SPEED, Mathf.Abs(xInput));
        SetBoolSafe(P_GROUNDED, grounded);
        SetFloatSafe(P_YVEL, rb.velocity.y);

        if (!wasGrounded && grounded)
            SetTriggerSafe(T_LAND);

        if (GetBoolSafe(P_ISBUSY) && Time.time > busyUntil)
            EndBusy();

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded && !GetBoolSafe(P_ISBUSY))
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // actions
        if (Input.GetKeyDown(KeyCode.J)) TryTrigger(T_ATTACK, ActionType.Attack);
        if (Input.GetKeyDown(KeyCode.Q)) TryTrigger(T_SKILLQ, ActionType.SkillQ);
        if (Input.GetKeyDown(KeyCode.E)) TryTrigger(T_SKILLE, ActionType.SkillE);
    }

    void FixedUpdate()
    {
        if (!GetBoolSafe(P_ISBUSY))
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    bool IsGrounded()
    {
        if (groundCheck == null)
        {
            if (!warnedMissing)
            {
                Debug.LogWarning("groundCheck NULL: kéo GroundCheck vào Inspector.");
                warnedMissing = true;
            }
            return false;
        }
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void TryTrigger(string triggerName, ActionType actionType)
    {
        if (GetBoolSafe(P_ISBUSY)) return;

        currentAction = actionType;
        busyUntil = Time.time + actionTimeout;

        SetBoolSafe(P_ISBUSY, true);
        ResetTriggerSafe(triggerName);
        SetTriggerSafe(triggerName);
    }

    // =================== Animation Events ===================
    public void AnimEvent_EndBusy() => EndBusy();

    // BạGetComponentInParent<EnemyHealth1>()n đặt event “bung chiêu” gọi 2 hàm này:
    public void AnimEvent_SpawnFire()
    {
        SpawnVFX(firePrefab);
        DealDamageAtSpawn(); // ✅ gây damage
    }

    public void AnimEvent_SpawnLightning()
    {
        SpawnVFX(lightningPrefab);
        DealDamageAtSpawn(); // ✅ gây damage
    }

    // Nếu pack clip gọi tên event khác:
    public void AE_SkillQ_Hit() => AnimEvent_SpawnFire();
    public void AE_SkillE_Hit() => AnimEvent_SpawnLightning();

    public void AE_SkillQ_End() => EndBusy();
    public void AE_SkillE_End() => EndBusy();
    public void AE_Attack_End() => EndBusy();

    void EndBusy()
    {
        SetBoolSafe(P_ISBUSY, false);
        currentAction = ActionType.None;
    }

    void SpawnVFX(GameObject prefab)
    {
        if (prefab == null || vfxSpawn == null) return;

        var go = Instantiate(prefab, vfxSpawn.position, Quaternion.identity);

        var s = go.transform.localScale;
        s.x = Mathf.Abs(s.x) * facing;
        go.transform.localScale = s;

        Destroy(go, vfxLifeTime);
    }

    void DealDamageAtSpawn()
    {
        if (vfxSpawn == null) return;

        var hits = Physics2D.OverlapCircleAll(vfxSpawn.position, skillRadius, enemyLayer);

        foreach (var h in hits)
        {
            var ehp = h.GetComponentInParent<EnemyHealth1>();
            if (ehp != null) ehp.TakeDamage(skillDamage);
        }
    }

    // =================== Safe wrappers ===================
    bool HasParam(string name) => paramNames.Contains(name);

    void SetFloatSafe(string name, float v) { if (HasParam(name)) anim.SetFloat(name, v); }
    void SetBoolSafe(string name, bool v) { if (HasParam(name)) anim.SetBool(name, v); }
    bool GetBoolSafe(string name) => HasParam(name) && anim.GetBool(name);
    void SetTriggerSafe(string name) { if (HasParam(name)) anim.SetTrigger(name); }
    void ResetTriggerSafe(string name) { if (HasParam(name)) anim.ResetTrigger(name); }

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