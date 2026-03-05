using System.Collections;
using UnityEngine;

public class EnemyRangedAttack2D : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public LayerMask playerLayer;
    public SimpleChaseRB chase;
    public Animator anim;

    [Header("Stats")]
    public float attackRange = 7f;
    public int damage = 6;
    public float projectileSpeed = 10f;
    public float cooldown = 1.5f;
    public float windupSeconds = 0.25f;

    [Header("VFX (optional)")]
    public GameObject muzzleVfxPrefab;
    public float muzzleVfxScale = 0.7f;
    public GameObject hitVfxPrefab;
    public float hitVfxScale = 0.9f;

    private float _nextTime;
    private Transform _player;
    private EnemyHealth _hp;

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
    }

    void Update()
    {
        if (StageResultUI.IsShowingResult) return;
        if (_hp != null && _hp.IsDead) return;
        if (_player == null) return;
        if (Time.time < _nextTime) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        if (dist > attackRange) return;

        StartCoroutine(CoShoot());
        _nextTime = Time.time + cooldown;
    }

    IEnumerator CoShoot()
    {
        if (chase) chase.enabled = false;
        if (anim) anim.SetTrigger("Attack");

        yield return new WaitForSeconds(windupSeconds);

        if (_hp != null && _hp.IsDead) yield break;
        if (_player == null) yield break;
        if (!projectilePrefab) { if (chase) chase.enabled = true; yield break; }

        // muzzle VFX
        if (muzzleVfxPrefab != null)
        {
            var v = Instantiate(muzzleVfxPrefab, firePoint.position, Quaternion.identity);
            v.transform.localScale = Vector3.one * muzzleVfxScale;
        }

        Vector2 dir = (_player.position - firePoint.position);
        dir = dir.sqrMagnitude < 0.0001f ? Vector2.right : dir.normalized;

        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var proj = go.GetComponent<EnemyProjectile2D>();
        if (proj != null)
            proj.Init(dir, projectileSpeed, damage, playerLayer, hitVfxPrefab, hitVfxScale);

        yield return new WaitForSeconds(0.15f);
        if (chase) chase.enabled = true;
    }
}