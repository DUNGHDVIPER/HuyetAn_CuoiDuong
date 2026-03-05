using System.Collections;
using UnityEngine;

public class PlayerStatusEffect2D : MonoBehaviour
{
    public PlayerController2D controller;
    public PlayerCombat2D combat;
    public Animator anim;

    [Header("Stun VFX (optional)")]
    public GameObject stunVfxPrefab;     // VFX_Lightning
    public float stunVfxScale = 1.2f;
    public float stunVfxInterval = 0.35f;
    public Vector3 stunVfxOffset = new Vector3(0f, 1.2f, 0f);

    private Coroutine _stunCo;
    private float _stunEndTime;

    void Awake()
    {
        if (!controller) controller = GetComponent<PlayerController2D>();
        if (!combat) combat = GetComponent<PlayerCombat2D>();
        if (!anim) anim = GetComponent<Animator>();
    }

    public void ApplyStun(float seconds)
    {
        if (seconds <= 0f) return;

        _stunEndTime = Mathf.Max(_stunEndTime, Time.time + seconds);

        if (_stunCo == null)
            _stunCo = StartCoroutine(CoStun());
    }

    IEnumerator CoStun()
    {
        if (controller) controller.enabled = false;
        if (combat) combat.enabled = false;
        if (anim) anim.SetBool("IsStunned", true);

        float nextFx = 0f;

        while (Time.time < _stunEndTime)
        {
            if (stunVfxPrefab != null && Time.time >= nextFx)
            {
                var v = Instantiate(stunVfxPrefab, transform.position + stunVfxOffset, Quaternion.identity);
                v.transform.localScale = Vector3.one * stunVfxScale;

                // kéo life theo interval để nhìn “nhấp nháy”
                var ad = v.GetComponent<AutoDestroy>();
                if (ad != null) ad.life = Mathf.Min(0.5f, stunVfxInterval);

                nextFx = Time.time + stunVfxInterval;
            }
            yield return null;
        }

        if (anim) anim.SetBool("IsStunned", false);
        if (controller) controller.enabled = true;
        if (combat) combat.enabled = true;

        _stunCo = null;
    }
}