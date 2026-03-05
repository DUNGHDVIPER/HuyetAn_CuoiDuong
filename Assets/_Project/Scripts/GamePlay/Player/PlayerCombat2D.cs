using UnityEngine;

public class PlayerCombat2D : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;
    public Transform attackPoint;
    public SpriteRenderer sr; // để lấy hướng (flipX)

    [Header("Hit (Normal Attack)")]
    public LayerMask enemyLayer;
    public float attackRange = 1.5f;
    public int attackDamage = 1;

    [Header("Combo")]
    public float comboResetTime = 0.8f;
    public float comboInputMin = 0.25f;
    public float comboInputMax = 0.80f;

    [Header("Block")]
    public bool isBlocking;

    [Header("Skills (Cooldown)")]
    public float skillQCooldown = 2.5f; // Q = Lightning
    public float skillECooldown = 3.0f; // E = Fire

    [Header("SkillQ (Lightning) AOE - Circle")]
    public float lightningRadius = 2.6f;
    public float lightningForwardOffset = 1.0f; // 0 = nổ quanh người
    public GameObject lightningVfxPrefab;
    public float lightningVfxScale = 2.2f;
    public int lightningDamage = 2;

    [Header("SkillE (Fire) AOE - Box")]
    public Vector2 fireBoxSize = new Vector2(4.0f, 2.2f);
    public float fireForwardOffset = 1.2f;
    public GameObject fireVfxPrefab;
    public float fireVfxScale = 1.8f;
    public int fireDamage = 2;

    int comboStep = 0;
    float lastAttackTime = -999f;
    bool queuedNext = false;
    bool didHitThisAnim = false;

    float nextQTime = 0;
    float nextETime = 0;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!sr) sr = GetComponent<SpriteRenderer>();

        if (!attackPoint)
        {
            var ap = transform.Find("AttackPoint");
            if (ap) attackPoint = ap;
        }
    }

    void Update()
    {
        // BLOCK (giữ chuột phải)
        isBlocking = Input.GetMouseButton(1);
        anim.SetBool("IsBlocking", isBlocking);
        if (isBlocking) return;

        // ATTACK (chuột trái)
        if (Input.GetMouseButtonDown(0))
            OnAttackPressed();

        // SKILL Q (Lightning)
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextQTime)
        {
            nextQTime = Time.time + skillQCooldown;
            anim.SetTrigger("DoSkillQ");
        }

        // SKILL E (Fire)
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextETime)
        {
            nextETime = Time.time + skillECooldown;
            anim.SetTrigger("DoSkillE");
        }

        HandleComboProgressAndHit();
    }
    void LateUpdate()
    {
        if (!attackPoint || !sr) return;

        Vector3 p = attackPoint.localPosition;
        float absX = Mathf.Abs(p.x);
        p.x = sr.flipX ? -absX : absX;
        attackPoint.localPosition = p;
    }
    // ---------------------------
    // Normal Attack Combo
    // ---------------------------
    void OnAttackPressed()
    {
        if (Time.time - lastAttackTime > comboResetTime)
            comboStep = 0;

        lastAttackTime = Time.time;

        if (IsInAttackState())
        {
            queuedNext = true;
            return;
        }

        comboStep = 1;
        FireAttack(comboStep);
    }

    void FireAttack(int step)
    {
        didHitThisAnim = false;
        queuedNext = false;

        anim.SetInteger("ComboStep", step);
        anim.SetTrigger("DoAttack");
    }

    void HandleComboProgressAndHit()
    {
        if (!IsInAttackState()) return;

        var st = anim.GetCurrentAnimatorStateInfo(0);
        float t = st.normalizedTime % 1f;

        if (!didHitThisAnim && t >= 0.40f)
        {
            didHitThisAnim = true;
            DoHit_Normal();
        }

        if (queuedNext && t >= comboInputMin && t <= comboInputMax)
        {
            queuedNext = false;
            comboStep = (comboStep < 3) ? comboStep + 1 : 1;
            FireAttack(comboStep);
        }
    }

    bool IsInAttackState()
    {
        var st = anim.GetCurrentAnimatorStateInfo(0);
        return st.IsTag("Attack");
    }

    // ---------------------------
    // Damage + Feedback Helpers (NEW)
    // ---------------------------
    void ApplyDamageAndFeedback(Collider2D col, Vector3 refPoint, int damage)
    {
        if (col == null) return;

        // hitPoint để feedback dùng (nếu script có)
        Vector2 hitPoint = col.ClosestPoint(refPoint);

        // 1) Damage (ưu tiên InParent vì collider hay nằm ở child)
        var hp = col.GetComponentInParent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        // 2) Feedback (component riêng, không nằm trong EnemyHealth)
        var fb = col.GetComponentInParent<EnemyHitFeedback2D>();
        if (fb != null)
        {
            // Nếu script của bạn có method khác tên, báo mình, mình chỉnh đúng theo file.
            fb.PlayHitFeedback(hitPoint);
        }
    }

    void DoHit_Normal()
    {
        if (!attackPoint) return;

        var hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // Debug nếu cần
        // Debug.Log($"Normal hits = {hits.Length}");

        foreach (var col in hits)
        {
            ApplyDamageAndFeedback(col, attackPoint.position, attackDamage);
        }
    }

    // ---------------------------
    // Skill AOE (Shared helpers)
    // ---------------------------
    int FacingSign()
    {
        // Nếu bạn dùng flipX để quay hướng:
        if (sr) return sr.flipX ? -1 : 1;

        // fallback: scale
        return transform.localScale.x < 0 ? -1 : 1;
    }

    Vector3 ForwardOffsetPos(float offset)
    {
        int dir = FacingSign();
        return attackPoint
            ? attackPoint.position + new Vector3(dir * offset, 0f, 0f)
            : transform.position + new Vector3(dir * offset, 0f, 0f);
    }

    void SpawnVfx(GameObject prefab, Vector3 pos, float scale)
    {
        if (!prefab) return;
        var go = Instantiate(prefab, pos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
    }

    // SkillQ: Lightning (Circle AOE)
    void DoAOE_Lightning()
    {
        Vector3 center = ForwardOffsetPos(lightningForwardOffset);

        var hits = Physics2D.OverlapCircleAll(center, lightningRadius, enemyLayer);
        Debug.Log($"Lightning AOE hits = {hits.Length}");

        foreach (var col in hits)
        {
            ApplyDamageAndFeedback(col, center, lightningDamage);
        }

        SpawnVfx(lightningVfxPrefab, center, lightningVfxScale);
    }

    // SkillE: Fire (Box AOE)
    void DoAOE_Fire()
    {
        Vector3 center = ForwardOffsetPos(fireForwardOffset);

        var hits = Physics2D.OverlapBoxAll(center, fireBoxSize, 0f, enemyLayer);
        Debug.Log($"Fire AOE hits = {hits.Length}");

        foreach (var col in hits)
        {
            ApplyDamageAndFeedback(col, center, fireDamage);
        }

        SpawnVfx(fireVfxPrefab, center, fireVfxScale);
    }

    // ---------------------------
    // Animation Events (IMPORTANT)
    // Đặt event này trên CLIP PLAYER (Anim_SkillQ... / Anim_SkillE...)
    // KHÔNG đặt trên clip VFX
    // ---------------------------
    public void AE_SkillQ_Hit() => DoAOE_Lightning();
    public void AE_SkillE_Hit() => DoAOE_Fire();

    // ---------------------------
    // Gizmos
    // ---------------------------
    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.cyan;
        Vector3 lCenter = Application.isPlaying
            ? ForwardOffsetPos(lightningForwardOffset)
            : attackPoint.position + new Vector3((sr && sr.flipX ? -1 : 1) * lightningForwardOffset, 0f, 0f);
        Gizmos.DrawWireSphere(lCenter, lightningRadius);
        Gizmos.DrawWireSphere(lCenter, lightningRadius);

        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Vector3 fCenter = Application.isPlaying
            ? ForwardOffsetPos(fireForwardOffset)
            : attackPoint.position + new Vector3((sr && sr.flipX ? -1 : 1) * fireForwardOffset, 0f, 0f);
        Gizmos.DrawWireCube(fCenter, fireBoxSize);
    }
}