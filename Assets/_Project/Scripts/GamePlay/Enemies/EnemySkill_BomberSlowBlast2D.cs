using System.Collections;
using UnityEngine;

public class EnemySkill_BomberSlowBlast2D : MonoBehaviour
{
    [Header("Refs")]
    public SimpleChaseRB chase;
    public Animator anim;
    public LayerMask playerLayer;

    [Header("Skill")]
    public float triggerDistance = 6f;
    public float windupSeconds = 1.25f;
    public float blastRadius = 2.2f;
    public int damage = 22;
    public float cooldown = 7f;

    [Header("VFX (optional)")]
    public GameObject telegraphVfxPrefab;   // VFX_TelegraphCircle
    public float telegraphScale = 2.2f;
    public GameObject explosionVfxPrefab;   // VFX_Fire
    public float explosionVfxScale = 2.4f;

    private float _nextTime;
    private Transform _player;
    private EnemyHealth _hp;

    void Awake()
    {
        if (!chase) chase = GetComponent<SimpleChaseRB>();
        if (!anim) anim = GetComponent<Animator>();
        _hp = GetComponent<EnemyHealth>();
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
        if (dist > triggerDistance) return;

        StartCoroutine(CoBlast());
        _nextTime = Time.time + cooldown;
    }

    IEnumerator CoBlast()
    {
        if (chase) chase.enabled = false;
        if (anim) anim.SetTrigger("Skill");

        Vector3 targetPos = _player.position;

        // telegraph
        GameObject tg = null;
        if (telegraphVfxPrefab != null)
        {
            tg = Instantiate(telegraphVfxPrefab, targetPos, Quaternion.identity);
            tg.transform.localScale = Vector3.one * telegraphScale;

            var ad = tg.GetComponent<AutoDestroy>();
            if (ad != null) ad.life = windupSeconds;
        }

        yield return new WaitForSeconds(windupSeconds);

        // explosion vfx
        if (explosionVfxPrefab != null)
        {
            var ex = Instantiate(explosionVfxPrefab, targetPos, Quaternion.identity);
            ex.transform.localScale = Vector3.one * explosionVfxScale;
        }

        Collider2D hit = Physics2D.OverlapCircle(targetPos, blastRadius, playerLayer);
        if (hit != null)
        {
            var ph = hit.GetComponentInParent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.2f);
        if (chase) chase.enabled = true;
    }
}