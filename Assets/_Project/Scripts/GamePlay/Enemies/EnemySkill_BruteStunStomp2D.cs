using System.Collections;
using UnityEngine;

public class EnemySkill_BruteStunStomp2D : MonoBehaviour
{
    [Header("Refs")]
    public SimpleChaseRB chase;
    public Animator anim;
    public LayerMask playerLayer;

    [Header("Skill")]
    public float triggerDistance = 3f;
    public float windupSeconds = 0.55f;
    public float stompRadius = 2.1f;
    public int damage = 3;
    public float stunSeconds = 1.2f;
    public float cooldown = 8f;

    [Header("VFX (optional)")]
    public GameObject windupVfxPrefab;  // VFX_TelegraphCircle
    public float windupVfxScale = 2.0f;
    public GameObject stompVfxPrefab;   // VFX_Lightning
    public float stompVfxScale = 1.6f;

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

        StartCoroutine(CoStomp());
        _nextTime = Time.time + cooldown;
    }

    IEnumerator CoStomp()
    {
        if (chase) chase.enabled = false;
        if (anim) anim.SetTrigger("Skill");

        // telegraph ngay tại brute
        if (windupVfxPrefab != null)
        {
            var tg = Instantiate(windupVfxPrefab, transform.position, Quaternion.identity);
            tg.transform.localScale = Vector3.one * windupVfxScale;
            var ad = tg.GetComponent<AutoDestroy>();
            if (ad != null) ad.life = windupSeconds;
        }

        yield return new WaitForSeconds(windupSeconds);

        // stomp vfx
        if (stompVfxPrefab != null)
        {
            var v = Instantiate(stompVfxPrefab, transform.position, Quaternion.identity);
            v.transform.localScale = Vector3.one * stompVfxScale;
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, stompRadius, playerLayer);
        if (hit != null)
        {
            var ph = hit.GetComponentInParent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);

            var status = hit.GetComponentInParent<PlayerStatusEffect2D>();
            if (status != null) status.ApplyStun(stunSeconds);
        }

        yield return new WaitForSeconds(0.25f);
        if (chase) chase.enabled = true;
    }
}