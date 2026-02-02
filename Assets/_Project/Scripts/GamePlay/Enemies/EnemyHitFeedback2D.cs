using System.Collections;
using UnityEngine;

public class EnemyHitFeedback2D : MonoBehaviour
{
    [Header("Flash")]
    public SpriteRenderer sr;
    public Color hitColor = new Color(1f, 0.25f, 0.25f, 1f);
    public float flashDuration = 0.10f;

    [Header("Shake")]
    public float shakeDuration = 0.12f;
    public float shakeStrength = 0.06f;

    [Header("VFX")]
    public GameObject hitVfxPrefab;      // prefab VFX (spark/slash)
    public Vector2 vfxOffset = Vector2.zero;

    Coroutine co;

    void Awake()
    {
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void PlayHitFeedback(Vector2 hitPoint)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(CoHit(hitPoint));
    }

    IEnumerator CoHit(Vector2 hitPoint)
    {
        // 1) Flash
        Color original = sr ? sr.color : Color.white;
        if (sr) sr.color = hitColor;

        // 2) VFX spawn
        if (hitVfxPrefab)
        {
            Vector3 pos = new Vector3(hitPoint.x, hitPoint.y, 0) + (Vector3)vfxOffset;
            Instantiate(hitVfxPrefab, pos, Quaternion.identity);
        }

        // 3) Shake
        Vector3 startLocal = transform.localPosition;
        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float x = Random.Range(-shakeStrength, shakeStrength);
            float y = Random.Range(-shakeStrength, shakeStrength);
            transform.localPosition = startLocal + new Vector3(x, y, 0);
            yield return null;
        }
        transform.localPosition = startLocal;

        // end flash
        yield return new WaitForSeconds(flashDuration);
        if (sr) sr.color = original;
    }
}
