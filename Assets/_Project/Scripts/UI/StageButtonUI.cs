using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageButtonUI : MonoBehaviour
{
    public enum State { Locked, Unlocked, Boss }

    [Header("Refs")]
    public Image bg;
    public Image border;
    public Image glow;

    public GameObject lockGroup;
    public Image lockDim;
    public Image lockIcon;

    public GameObject bossGroup; // smoke/vfx group
    public TMP_Text label;

    [Header("Colors")]
    public Color unlockedBorder = new Color32(0x7A, 0x1F, 0x1F, 0xFF);
    public Color lockedBorder = new Color32(0x22, 0x22, 0x22, 0xFF);
    public Color bossBorder = new Color32(0xFF, 0x2A, 0x2A, 0xFF);

    public Color unlockedText = new Color32(0xE0, 0xE0, 0xE0, 0xFF);
    public Color bossText = new Color32(0xFF, 0x3B, 0x3B, 0xFF);

    Button _btn;

    void Awake()
    {
        _btn = GetComponent<Button>();
    }

    public void SetText(string txt)
    {
        if (label) label.text = txt;
    }

    public void SetState(State state)
    {
        if (_btn == null) _btn = GetComponent<Button>();

        switch (state)
        {
            case State.Locked:
                if (lockGroup) lockGroup.SetActive(true);
                if (bossGroup) bossGroup.SetActive(false);
                if (glow) SetAlpha(glow, 0f);
                if (border) border.color = lockedBorder;
                if (label) { label.color = unlockedText; label.alpha = 0.35f; }
                if (_btn) _btn.interactable = false;
                break;

            case State.Unlocked:
                if (lockGroup) lockGroup.SetActive(false);
                if (bossGroup) bossGroup.SetActive(false);
                if (glow) SetAlpha(glow, 0f);
                if (border) border.color = unlockedBorder;
                if (label) { label.color = unlockedText; label.alpha = 1f; }
                if (_btn) _btn.interactable = true;
                break;

            case State.Boss:
                if (lockGroup) lockGroup.SetActive(false);
                if (bossGroup) bossGroup.SetActive(true);
                if (glow) SetAlpha(glow, 0.75f);
                if (border) border.color = bossBorder;
                if (label) { label.color = bossText; label.alpha = 1f; }
                if (_btn) _btn.interactable = true;
                break;
        }
    }

    static void SetAlpha(Graphic g, float a)
    {
        var c = g.color;
        c.a = a;
        g.color = c;
    }
}
