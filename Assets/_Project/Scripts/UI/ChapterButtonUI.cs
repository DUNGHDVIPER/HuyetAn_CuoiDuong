using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ChapterButtonUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum State { Locked, Unlocked, Boss }

    [Header("Refs")]
    public Image bg;
    public Image border;
    public Image glow;
    public GameObject iconLock;
    public GameObject smoke;
    public TMP_Text label;

    [Header("Tuning")]
    public float hoverScale = 1.06f;
    public float pressScale = 0.93f;
    public float speed = 14f;

    [Header("Colors")]
    public Color bgLocked = new Color(0.05f, 0.06f, 0.08f, 0.75f);
    public Color bgUnlocked = new Color(0.06f, 0.07f, 0.10f, 0.95f);

    public Color borderLocked = new Color(0.20f, 0.20f, 0.22f, 0.75f);
    public Color borderUnlocked = new Color(0.35f, 0.10f, 0.10f, 0.95f);

    public Color textLocked = new Color(0.75f, 0.75f, 0.78f, 0.35f);
    public Color textUnlocked = new Color(0.92f, 0.92f, 0.92f, 1f);
    public Color textBoss = new Color(1f, 0.25f, 0.25f, 1f);

    State state;
    Vector3 baseScale;
    float glowTarget;
    float scaleTarget = 1f;

    void Awake()
    {
        baseScale = transform.localScale;
        if (glow) { var c = glow.color; c.a = 0f; glow.color = c; }
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, baseScale * scaleTarget, Time.unscaledDeltaTime * speed);

        if (glow)
        {
            var c = glow.color;
            c.a = Mathf.Lerp(c.a, glowTarget, Time.unscaledDeltaTime * speed);
            glow.color = c;
        }
    }

    public void SetText(string t)
    {
        if (label) label.text = t;
    }

    public void SetState(State s)
    {
        state = s;

        if (iconLock) iconLock.SetActive(s == State.Locked);
        if (smoke) smoke.SetActive(s == State.Boss);

        if (bg) bg.color = (s == State.Locked) ? bgLocked : bgUnlocked;
        if (border) border.color = (s == State.Locked) ? borderLocked : borderUnlocked;

        if (label)
        {
            label.color = (s == State.Locked) ? textLocked : (s == State.Boss ? textBoss : textUnlocked);
        }

        // Boss glow luôn bật nhẹ
        glowTarget = (s == State.Boss) ? 0.75f : 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state == State.Locked) return;
        scaleTarget = hoverScale;
        glowTarget = (state == State.Boss) ? 0.9f : 0.55f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (state == State.Locked) return;
        scaleTarget = 1f;
        glowTarget = (state == State.Boss) ? 0.75f : 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (state == State.Locked) return;
        scaleTarget = pressScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (state == State.Locked) return;
        scaleTarget = hoverScale;
    }
}
