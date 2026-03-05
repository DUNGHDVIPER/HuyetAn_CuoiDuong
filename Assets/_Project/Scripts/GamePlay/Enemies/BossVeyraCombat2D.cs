using System.Collections;
using UnityEngine;

public class BossVeyraCombat2D : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public LayerMask playerLayer;
    public SimpleChaseRB chase;
    public Animator anim;

    [Header("Attack #2 - Heavy Strike")]
    public float heavyRange = 3.0f;
    public int heavyDamage = 18;
    public float heavyCooldown = 3.2f;
    public float heavyWindup = 0.55f;

    [Header("Skill - Rift Bolt (projectile)")]
    public float boltRange = 9f;
    public int boltDamage = 14;
    public float boltSpeed = 10f;
    public float boltCooldown = 5.0f;
    public float boltWindup = 0.35f;

    [Header("Ultimate - Core Burst (AOE)")]
    public int ultiDamage = 28;
    public float ultiRadius = 3.2f;
    public float ultiWindup = 0.8f;
    public float ultiCooldownMin = 18f;
    public float ultiCooldownMax = 22f;

    [Header("VFX (optional)")]
    public GameObject boltCastVfxPrefab;       // VFX_Lightning
    public float boltCastVfxScale = 1.0f;

    public GameObject boltHitVfxPrefab;        // VFX_Lightning (spark on hit)
    public float boltHitVfxScale = 1.0f;

    public GameObject ultiTelegraphVfxPrefab;  // VFX_TelegraphCircle
    public float ultiTelegraphScale = 3.2f;

    public GameObject ultiExplosionVfxPrefab;  // VFX_Fire
    public float ultiExplosionVfxScale = 3.2f;

    public GameObject heavyVfxPrefab;          // optional (VFX_Lightning / slash)
    public float heavyVfxScale = 1.4f;
    public Vector3 heavyVfxOffset = new Vector3(0f, 0.2f, 0f);

    private EnemyHealth _hp;
    private Transform _player;
    private float _nextHeavy;
    private float _nextBolt;
    private float _nextUlti;
    private bool _casting;

    void Awake()
    {
        if (!chase) chase = GetComponent<SimpleChaseRB>();
        if (!anim) anim = GetComponent<Animator>();
        _hp = GetComponent<EnemyHealth>();

        if (!firePoint)
        {
            var fp = transform.Find("FirePoint");
            if (fp) firePoint = fp;
            else firePoint = transform;
        }
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;

        _nextUlti = Time.time + Random.Range(ultiCooldownMin, ultiCooldownMax);
    }

    void Update()
    {
        if (StageResultUI.IsShowingResult) return;
        if (_hp != null && _hp.IsDead) return;

        if (_player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) _player = p.transform;
            if (_player == null) return;
        }

        if (_casting) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        // Priority: Ulti (tự chạy timer)
        if (Time.time >= _nextUlti)
        {
            StartCoroutine(CoUltimate());
            _nextUlti = Time.time + Random.Range(ultiCooldownMin, ultiCooldownMax);
            return;
        }

        // Skill bolt
        if (dist <= boltRange && Time.time >= _nextBolt)
        {
            StartCoroutine(CoBolt());
            _nextBolt = Time.time + boltCooldown;
            return;
        }

        // Heavy strike
        if (dist <= heavyRange && Time.time >= _nextHeavy)
        {
            StartCoroutine(CoHeavy());
            _nextHeavy = Time.time + heavyCooldown;
            return;
        }
    }

    IEnumerator CoHeavy()
    {
        _casting = true;
        if (chase) chase.enabled = false;
        if (anim) anim.SetTrigger("Attack2");

        // optional: telegraph small VFX at boss
        if (heavyVfxPrefab != null)
        {
            var v = Instantiate(heavyVfxPrefab, transform.position + heavyVfxOffset, Quaternion.identity);
            v.transform.localScale = Vector3.one * heavyVfxScale;
        }

        yield return new WaitForSeconds(heavyWindup);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, heavyRange, playerLayer);
        if (hit != null)
        {
            var ph = hit.GetComponentInParent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(heavyDamage);
        }

        yield return new WaitForSeconds(0.25f);
        if (chase) chase.enabled = true;
        _casting = false;
    }

    IEnumerator CoBolt()
    {
        _casting = true;
        if (chase) chase.enabled = false;
        if (anim) anim.SetTrigger("Skill1");

        yield return new WaitForSeconds(boltWindup);

        if (!projectilePrefab || firePoint == null)
        {
            if (chase) chase.enabled = true;
            _casting = false;
            yield break;
        }

        // cast VFX at muzzle
        if (boltCastVfxPrefab != null)
        {
            var v = Instantiate(boltCastVfxPrefab, firePoint.position, Quaternion.identity);
            v.transform.localScale = Vector3.one * boltCastVfxScale;
        }

        Vector2 dir = (_player.position - firePoint.position);
        dir = dir.sqrMagnitude < 0.0001f ? Vector2.right : dir.normalized;

        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var proj = go.GetComponent<EnemyProjectile2D>();
        if (proj != null)
        {
            // Note: Init signature updated to accept hit VFX (see EnemyProjectile2D updated version)
            proj.Init(dir, boltSpeed, boltDamage, playerLayer, boltHitVfxPrefab, boltHitVfxScale);
        }

        yield return new WaitForSeconds(0.15f);
        if (chase) chase.enabled = true;
        _casting = false;
    }

    IEnumerator CoUltimate()
    {
        _casting = true;
        if (chase) chase.enabled = false;
        if (anim) anim.SetTrigger("Ulti");

        Vector3 targetPos = _player.position; // lock vị trí lúc cast

        // Telegraph VFX at target position
        GameObject tg = null;
        if (ultiTelegraphVfxPrefab != null)
        {
            tg = Instantiate(ultiTelegraphVfxPrefab, targetPos, Quaternion.identity);
            tg.transform.localScale = Vector3.one * ultiTelegraphScale;

            var ad = tg.GetComponent<AutoDestroy>();
            if (ad != null) ad.life = ultiWindup;
        }

        yield return new WaitForSeconds(ultiWindup);

        // Explosion VFX at target position
        if (ultiExplosionVfxPrefab != null)
        {
            var ex = Instantiate(ultiExplosionVfxPrefab, targetPos, Quaternion.identity);
            ex.transform.localScale = Vector3.one * ultiExplosionVfxScale;
        }

        Collider2D hit = Physics2D.OverlapCircle(targetPos, ultiRadius, playerLayer);
        if (hit != null)
        {
            var ph = hit.GetComponentInParent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(ultiDamage);
        }

        // downtime sau ulti ~1.8s theo spec
        yield return new WaitForSeconds(1.8f);

        if (chase) chase.enabled = true;
        _casting = false;
    }

    // Optional gizmo to visualize heavy/ulti
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, heavyRange);

        if (_player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_player.position, ultiRadius);
        }
    }
}