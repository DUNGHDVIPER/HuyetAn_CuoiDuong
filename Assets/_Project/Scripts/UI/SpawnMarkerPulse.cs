using UnityEngine;

public class SpawnMarkerPulse : MonoBehaviour
{
    public SpriteRenderer sr;

    [Header("Pulse")]
    public float pulseSpeed = 3f;
    public float minA = 0.2f;
    public float maxA = 0.6f;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!sr) return;

        float s = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed);
        var c = sr.color;
        c.a = Mathf.Lerp(minA, maxA, s);
        sr.color = c;
    }
}