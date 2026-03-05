using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Auto-created runtime TMP (no prefab needed)")]
    public TextMeshPro tmp3D;          // TMP 3D, runtime auto-create

    [Header("Look & Feel")]
    public float duration = 0.6f;      // fade time
    public float lifeTime = 0.8f;      // destroy after
    public float riseDistance = 1.2f;  // world units
    public float startScale = 1.2f;
    public float endScale = 1.0f;
    public bool useUnscaledTime = false;

    [Header("Sorting (auto apply)")]
    public string sortingLayerName = "UIWorld";
    public int sortingOrder = 50;

    [Header("Randomness")]
    public float randomX = 0.15f;
    public float randomY = 0.10f;

    private Coroutine _co;
    private Vector3 _startPos;

    // =========================
    // STATIC API (Cách 1)
    // =========================

    public static DamagePopup Spawn(int value, Vector3 worldPos, Color color, bool isCrit = false)
    {
        string text = isCrit ? $"{value}!" : value.ToString();
        return SpawnText(text, worldPos, color, isCrit);
    }

    public static DamagePopup SpawnText(string text, Vector3 worldPos, Color color, bool isCrit = false)
    {
        var popup = CreateRuntimePopup(worldPos);
        popup.Setup(text, color, isCrit);
        return popup;
    }

    // =========================
    // Instance
    // =========================

    private void Awake()
    {
        // safety: if created in editor/prefab (rare), try find TMP
        if (tmp3D == null) tmp3D = GetComponentInChildren<TextMeshPro>(true);
        ApplySorting();
    }

    private void ApplySorting()
    {
        // TMP 3D uses MeshRenderer (Renderer)
        var r = GetComponentInChildren<Renderer>(true);
        if (r != null)
        {
            if (!string.IsNullOrWhiteSpace(sortingLayerName))
                r.sortingLayerName = sortingLayerName;

            r.sortingOrder = sortingOrder;
        }
    }

    public void Setup(string text, Color color, bool isCrit)
    {
        // jitter pos (so multiple popups don't overlap perfectly)
        _startPos = transform.position;
        _startPos.x += Random.Range(-randomX, randomX);
        _startPos.y += Random.Range(-randomY, randomY);
        transform.position = _startPos;

        // set text
        if (tmp3D != null)
        {
            tmp3D.text = text;

            var c = color; c.a = 1f;
            tmp3D.color = c;

            // crit pop
            tmp3D.fontSize = isCrit ? 9.5f : 8f;
        }

        // run animation
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(Animate(color));
    }

    private IEnumerator Animate(Color baseColor)
    {
        float t = 0f;
        float fadeT = Mathf.Max(0.01f, duration);
        float totalLife = Mathf.Max(fadeT, lifeTime);

        transform.localScale = Vector3.one * startScale;

        while (t < totalLife)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;

            float k = Mathf.Clamp01(t / fadeT);

            // rise
            transform.position = _startPos + Vector3.up * (riseDistance * k);

            // scale
            float s = Mathf.Lerp(startScale, endScale, k);
            transform.localScale = Vector3.one * s;

            // fade alpha
            float a = Mathf.Lerp(1f, 0f, k);
            if (tmp3D != null)
            {
                var c = baseColor;
                c.a = a;
                tmp3D.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    // =========================
    // Runtime creation (NO prefab)
    // =========================

    private static DamagePopup CreateRuntimePopup(Vector3 pos)
    {
        var go = new GameObject("DamagePopup_Runtime");
        go.transform.position = pos;

        var popup = go.AddComponent<DamagePopup>();

        // child TMP 3D
        var tmpGo = new GameObject("TMP_Damage");
        tmpGo.transform.SetParent(go.transform, false);
        tmpGo.transform.localPosition = Vector3.zero;

        var tmp = tmpGo.AddComponent<TextMeshPro>();
        tmp.text = "0";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.fontSize = 8f;

        // outline for readability
        tmp.fontStyle = FontStyles.Bold;
        tmp.outlineWidth = 0.15f;
        tmp.outlineColor = new Color(0f, 0f, 0f, 0.85f);

        popup.tmp3D = tmp;

        // IMPORTANT: apply sorting after TMP exists
        popup.ApplySorting();

        return popup;
    }
}