using System.Collections;
using UnityEngine;

public class SpawnPortalFX : MonoBehaviour
{
    [Header("Renderers (drag & drop)")]
    public SpriteRenderer rune;
    public SpriteRenderer glow;
    public SpriteRenderer smoke;

    [Header("Idle Motion")]
    public float idleRotateSpeed = 45f;     // deg/sec
    public float smokeRotateSpeed = -25f;   // deg/sec (counter rotate)
    public float pulseSpeed = 3f;           // sin speed
    [Range(0f, 1f)] public float glowPulseMin = 0.25f;
    [Range(0f, 1f)] public float glowPulseMax = 0.75f;

    [Header("Ping / Telegraph")]
    public float pingDuration = 0.2f;
    [Range(0f, 2f)] public float pingGlowBoost = 1.0f;  // add alpha boost (clamped)
    [Range(0f, 2f)] public float pingRuneBoost = 0.35f;
    [Range(0f, 2f)] public float pingSmokeBoost = 0.25f;

    [Header("Timing")]
    public bool useUnscaledTime = false;

    private Color _runeBase;
    private Color _glowBase;
    private Color _smokeBase;

    private Coroutine _pingCo;

    void Awake()
    {
        // Auto-find if not assigned
        if (rune == null || glow == null || smoke == null)
        {
            var srs = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in srs)
            {
                if (sr == null) continue;
                var n = sr.name.ToLower();

                if (rune == null && (n.Contains("rune") || n.Contains("circle_rune") || n.Contains("ring")))
                    rune = sr;
                else if (glow == null && (n.Contains("glow") || n.Contains("circle_glow")))
                    glow = sr;
                else if (smoke == null && (n.Contains("smoke") || n.Contains("fog")))
                    smoke = sr;
            }
        }

        if (rune != null) _runeBase = rune.color;
        if (glow != null) _glowBase = glow.color;
        if (smoke != null) _smokeBase = smoke.color;
    }

    void OnEnable()
    {
        // restore base colors when enabled
        RestoreBaseColors();
    }

    void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        // idle rotate
        if (rune != null) rune.transform.Rotate(0, 0, idleRotateSpeed * dt);
        if (smoke != null) smoke.transform.Rotate(0, 0, smokeRotateSpeed * dt);

        // idle pulse glow alpha
        if (glow != null)
        {
            float s = 0.5f + 0.5f * Mathf.Sin((useUnscaledTime ? Time.unscaledTime : Time.time) * pulseSpeed);
            float a = Mathf.Lerp(glowPulseMin, glowPulseMax, s);

            var c = _glowBase;
            c.a = _glowBase.a * a;
            glow.color = c;
        }
    }

    public void Ping()
    {
        if (!isActiveAndEnabled) return;

        if (_pingCo != null) StopCoroutine(_pingCo);
        _pingCo = StartCoroutine(PingRoutine());
    }

    private IEnumerator PingRoutine()
    {
        float t = 0f;
        float dur = Mathf.Max(0.01f, pingDuration);

        // snapshot base colors (in case changed)
        if (rune != null) _runeBase = rune.color;
        if (glow != null) _glowBase = glow.color;
        if (smoke != null) _smokeBase = smoke.color;

        while (t < dur)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;

            float k = 1f - Mathf.Clamp01(t / dur); // 1 -> 0
            float boost = k; // strongest at start

            if (glow != null)
            {
                var c = _glowBase;
                c.a = Mathf.Clamp01(_glowBase.a + pingGlowBoost * boost);
                glow.color = c;
            }

            if (rune != null)
            {
                var c = _runeBase;
                c.a = Mathf.Clamp01(_runeBase.a + pingRuneBoost * boost);
                rune.color = c;
            }

            if (smoke != null)
            {
                var c = _smokeBase;
                c.a = Mathf.Clamp01(_smokeBase.a + pingSmokeBoost * boost);
                smoke.color = c;
            }

            yield return null;
        }

        RestoreBaseColors();
        _pingCo = null;
    }

    private void RestoreBaseColors()
    {
        if (rune != null) rune.color = _runeBase;
        if (glow != null) glow.color = _glowBase;
        if (smoke != null) smoke.color = _smokeBase;
    }
}