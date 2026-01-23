using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Refs")]
    public Image background;
    public Image hoverBar;
    public TMP_Text label;

    [Header("Colors")]
    public Color normalBg = new Color(0.10f, 0.10f, 0.11f, 1f);   // #1A1A1D
    public Color hoverBg = new Color(0.23f, 0.05f, 0.05f, 1f);   // đỏ thẫm
    public Color normalText = new Color(0.93f, 0.93f, 0.93f, 1f);
    public Color hoverText = Color.white;

    [Header("Motion")]
    public float hoverScale = 1.03f;
    public float pressScale = 0.98f;
    public float speed = 14f;

    Vector3 baseScale;
    float barAlphaTarget;
    float barFillTarget;   // để bar "chạy"
    float barFill;         // 0..1
    Color bgTarget;
    Color textTarget;

    RectTransform barRect;

    void Awake()
    {
        baseScale = transform.localScale;
        bgTarget = normalBg;
        textTarget = normalText;
        barAlphaTarget = 0f;
        barFillTarget = 0f;

        if (hoverBar != null)
            barRect = hoverBar.rectTransform;

        ApplyImmediate();
    }

    void Update()
    {
        // scale
        float targetScale = (barAlphaTarget > 0f) ? hoverScale : 1f;
        transform.localScale = Vector3.Lerp(transform.localScale, baseScale * targetScale, Time.unscaledDeltaTime * speed);

        // colors
        if (background != null)
            background.color = Color.Lerp(background.color, bgTarget, Time.unscaledDeltaTime * speed);

        if (label != null)
            label.color = Color.Lerp(label.color, textTarget, Time.unscaledDeltaTime * speed);

        // bar alpha
        if (hoverBar != null)
        {
            var c = hoverBar.color;
            c.a = Mathf.Lerp(c.a, barAlphaTarget, Time.unscaledDeltaTime * speed);
            hoverBar.color = c;

            // bar chạy ngang: scale X từ 0 -> 1
            barFill = Mathf.Lerp(barFill, barFillTarget, Time.unscaledDeltaTime * speed);
            if (barRect != null)
            {
                barRect.localScale = new Vector3(Mathf.Max(0.05f, barFill), 1f, 1f);
            }
        }
    }

    void ApplyImmediate()
    {
        if (background != null) background.color = normalBg;
        if (label != null) label.color = normalText;

        if (hoverBar != null)
        {
            var c = hoverBar.color;
            c.a = 0f;
            hoverBar.color = c;

            if (barRect != null)
                barRect.localScale = new Vector3(0.05f, 1f, 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bgTarget = hoverBg;
        textTarget = hoverText;
        barAlphaTarget = 0.9f;
        barFillTarget = 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bgTarget = normalBg;
        textTarget = normalText;
        barAlphaTarget = 0f;
        barFillTarget = 0.05f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = baseScale * pressScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // trả lại scale hover
        transform.localScale = baseScale * ((barAlphaTarget > 0f) ? hoverScale : 1f);
    }
}
