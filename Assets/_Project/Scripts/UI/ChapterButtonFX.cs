using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ChapterButtonFX : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    public enum State { Locked, Unlocked, Boss }

    [Header("Refs")]
    public RectTransform root;   // thường là chính nút (RectTransform)
    public Image bg;
    public Image border;
    public Image glow;
    public Image lockIcon;
    public Image smoke;          // optional
    public TMP_Text label;

    [Header("State")]
    public State state = State.Unlocked;

    [Header("Colors")]
    public Color bgLocked = new Color(0.08f, 0.08f, 0.09f, 1f);
    public Color bgNormal = new Color(0.12f, 0.12f, 0.13f, 1f);
    public Color bgHover = new Color(0.18f, 0.05f, 0.05f, 1f); // đỏ thẫm

    public Color borderLocked = new Color(0.12f, 0.12f, 0.12f, 1f);
    public Color borderNormal = new Color(0.45f, 0.12f, 0.12f, 1f);
    public Color borderHover = new Color(1f, 0.25f, 0.20f, 1f);

    public Color textLocked = new Color(0.75f, 0.75f, 0.75f, 0.45f);
    public Color textNormal = new Color(0.95f, 0.95f, 0.95f, 1f);
    public Color textBoss = new Color(1f, 0.23f, 0.18f, 1f);

    [Header("Glow")]
    [Range(0f, 1f)] public float glowIdleAlpha = 0.0f;     // chương thường: tắt
    [Range(0f, 1f)] public float glowHoverAlpha = 0.65f;
    [Range(0f, 1f)] public float glowBossAlpha = 0.80f;    // boss: luôn bật

    [Header("Motion")]
    public float hoverScale = 1.08f;
    public float pressScale = 0.92f;
    public float speed = 16f;

    [Header("Boss Pulse")]
    public bool bossPulse = true;
    public float pulseSpeed = 2.2f;
    public float pulseAmount = 0.06f; // 6%

    [Header("Click Flash")]
    public bool clickFlash = true;
    public float flashTime = 0.09f;

    // internal
    Vector3 baseScale;
    bool isHover;
    bool isPress;
    float glowTarget;
    float flashT;
    Color bgTarget, borderTarget, textTarget;

    void Reset()
    {
        root = GetComponent<RectTransform>();
        var imgs = GetComponentsInChildren<Image>(true);
        foreach (var i in imgs)
        {
            if (i.name.ToLower().Contains("bg")) bg = i;
            if (i.name.ToLower().Contains("border")) border = i;
            if (i.name.ToLower().Contains("glow")) glow = i;
            if (i.name.ToLower().Contains("lock")) lockIcon = i;
            if (i.name.ToLower().Contains("smoke")) smoke = i;
        }
        label = GetComponentInChildren<TMP_Text>(true);
    }

    void Awake()
    {
        if (root == null) root = GetComponent<RectTransform>();
        baseScale = root.localScale;

        ApplyStateVisual(immediate: true);
    }

    void OnEnable()
    {
        ApplyStateVisual(immediate: true);
    }

    void Update()
    {
        // Boss pulse (nhấp nhô nhẹ)
        float bossPulseScale = 1f;
        if (state == State.Boss && bossPulse && !isPress)
        {
            bossPulseScale = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;
        }

        float targetScale = 1f;

        if (isPress) targetScale = pressScale;
        else if (isHover && state != State.Locked) targetScale = hoverScale;

        root.localScale = Vector3.Lerp(root.localScale, baseScale * targetScale * bossPulseScale, Time.unscaledDeltaTime * speed);

        // Lerp màu
        if (bg) bg.color = Color.Lerp(bg.color, bgTarget, Time.unscaledDeltaTime * speed);
        if (border) border.color = Color.Lerp(border.color, borderTarget, Time.unscaledDeltaTime * speed);
        if (label) label.color = Color.Lerp(label.color, textTarget, Time.unscaledDeltaTime * speed);

        // Glow alpha
        if (glow)
        {
            var c = glow.color;
            c.a = Mathf.Lerp(c.a, glowTarget, Time.unscaledDeltaTime * speed);
            glow.color = c;
        }

        // Click flash: đẩy glow lên nhanh 1 nhịp
        if (clickFlash && flashT > 0f && glow)
        {
            flashT -= Time.unscaledDeltaTime;
            var c = glow.color;
            c.a = Mathf.Clamp01(c.a + 0.35f);
            glow.color = c;
        }
    }

    public void SetState(State s)
    {
        state = s;
        ApplyStateVisual(immediate: false);
    }

    public void SetText(string t)
    {
        if (label) label.text = t;
    }

    void ApplyStateVisual(bool immediate)
    {
        if (state == State.Locked)
        {
            bgTarget = bgLocked;
            borderTarget = borderLocked;
            textTarget = textLocked;
            glowTarget = 0f;

            if (lockIcon) lockIcon.gameObject.SetActive(true);
            if (smoke) smoke.gameObject.SetActive(false);
        }
        else if (state == State.Boss)
        {
            bgTarget = bgNormal;
            borderTarget = borderHover; // boss nổi bật hơn
            textTarget = textBoss;
            glowTarget = glowBossAlpha;

            if (lockIcon) lockIcon.gameObject.SetActive(false);
            if (smoke) smoke.gameObject.SetActive(true);
        }
        else // Unlocked
        {
            bgTarget = bgNormal;
            borderTarget = borderNormal;
            textTarget = textNormal;
            glowTarget = glowIdleAlpha;

            if (lockIcon) lockIcon.gameObject.SetActive(false);
            if (smoke) smoke.gameObject.SetActive(false);
        }

        if (immediate)
        {
            if (bg) bg.color = bgTarget;
            if (border) border.color = borderTarget;
            if (label) label.color = textTarget;
            if (glow)
            {
                var c = glow.color;
                c.a = glowTarget;
                glow.color = c;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;

        if (state == State.Locked) return;

        bgTarget = bgHover;
        borderTarget = borderHover;
        glowTarget = (state == State.Boss) ? glowBossAlpha : glowHoverAlpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        isPress = false;
        ApplyStateVisual(immediate: false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (state == State.Locked) return;

        isPress = true;
        if (clickFlash) flashT = flashTime;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPress = false;
        // trở về hover hoặc idle
        if (isHover) OnPointerEnter(eventData);
        else ApplyStateVisual(immediate: false);
    }
}
