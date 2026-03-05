using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageHUD : MonoBehaviour
{
    public static StageHUD Main { get; private set; }

    [Header("Title")]
    public TextMeshProUGUI title;

    [Header("Gameplay HUD (optional refs)")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bossHpText;
    public TextMeshProUGUI warningText;

    [Header("Bars (optional)")]
    public Image playerHpFill;          // Filled Horizontal
    public Image bossHpFill;            // Filled Horizontal
    public GameObject bossPanelRoot;    // Panel_BossHP (ẩn mặc định trong prefab)

    [Header("Damage Flash (optional)")]
    public Image damageFlashImage;      // Fullscreen red overlay (alpha 0 mặc định)
    [Range(0f, 1f)] public float damageFlashDefaultAlpha = 0.55f;
    public float damageFlashFadeOut = 0.12f;
    public bool flashUsesUnscaledTime = true;

    [Header("Low HP Effect")]
    [Range(0f, 1f)] public float lowHpThreshold = 0.30f;
    public float lowHpPulseSpeed = 6.0f;
    [Range(0f, 1f)] public float lowHpMinAlphaMultiplier = 0.35f;

    [Header("Smoothing")]
    public float hpFillLerpSpeed = 12f;
    public float bossFillLerpSpeed = 10f;

    private PlayerHealth _playerHp;
    private Coroutine _warningCo;
    private Coroutine _flashCo;

    private float _hpFillShown = 1f;
    private float _bossFillShown = 1f;

    private int _bossCurrent;
    private int _bossMax;

    private Color _playerFillBaseColor;
    private Color _bossFillBaseColor;

    private void Awake()
    {
        // Pick a "main" HUD instance (helpful when scene has old HUD refs null)
        if (Main == null) Main = this;
        else if (Main != this && Main.playerHpFill == null && this.playerHpFill != null) Main = this;

        if (playerHpFill != null) _playerFillBaseColor = playerHpFill.color;
        if (bossHpFill != null) _bossFillBaseColor = bossHpFill.color;
    }

    private void OnDestroy()
    {
        if (Main == this) Main = null;
    }

    private void Start()
    {
        // ====== giữ nguyên hành vi title (mở rộng parse boss) ======
        string scene = SceneManager.GetActiveScene().name; // "CH5_S1" / "CH5_B1"
        if (title != null)
        {
            title.text = scene;

            if (scene.StartsWith("CH"))
            {
                int us = scene.IndexOf('_');
                if (us > 2)
                {
                    string chStr = scene.Substring(2, us - 2);

                    if (scene.Contains("_S"))
                    {
                        int sIdx = scene.IndexOf("_S");
                        string sStr = (sIdx >= 0 && sIdx + 2 < scene.Length) ? scene.Substring(sIdx + 2) : "?";
                        title.text = $"Chapter {chStr} - Stage {sStr}";
                    }
                    else if (scene.Contains("_B")) // CH5_B1, CH5_BOSS...
                    {
                        title.text = $"Chapter {chStr} - BOSS";
                    }
                }
            }
        }

        CachePlayer();

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        // Boss panel mặc định ẩn
        if (bossPanelRoot != null)
            bossPanelRoot.SetActive(false);

        // Flash overlay alpha = 0
        if (damageFlashImage != null)
        {
            var c = damageFlashImage.color;
            c.a = 0f;
            damageFlashImage.color = c;
        }
    }

    private void Update()
    {
        UpdatePlayerHpUI();
        UpdateBossHpUI();
    }

    private void UpdatePlayerHpUI()
    {
        if (hpText == null && playerHpFill == null) return;

        if (_playerHp == null) CachePlayer();
        if (_playerHp == null) return;

        int cur = _playerHp.currentHP;
        int max = Mathf.Max(1, _playerHp.maxHP);
        float ratio = Mathf.Clamp01(cur / (float)max);

        if (hpText != null)
            hpText.text = $"HP: {cur}/{max}";

        if (playerHpFill != null)
        {
            // smooth fill
            _hpFillShown = Mathf.Lerp(_hpFillShown, ratio, 1f - Mathf.Exp(-hpFillLerpSpeed * Time.deltaTime));
            playerHpFill.fillAmount = _hpFillShown;

            // low HP pulse
            if (ratio <= lowHpThreshold)
            {
                float pulse01 = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * lowHpPulseSpeed);
                float aMul = Mathf.Lerp(lowHpMinAlphaMultiplier, 1f, pulse01);

                var c = _playerFillBaseColor;
                c.a = _playerFillBaseColor.a * aMul;
                playerHpFill.color = c;

                if (hpText != null)
                {
                    var tc = hpText.color;
                    tc.a = Mathf.Lerp(0.65f, 1f, pulse01);
                    hpText.color = tc;
                }
            }
            else
            {
                // restore
                playerHpFill.color = _playerFillBaseColor;
                if (hpText != null)
                {
                    var tc = hpText.color;
                    tc.a = 1f;
                    hpText.color = tc;
                }
            }
        }
    }

    private void UpdateBossHpUI()
    {
        if (bossHpText == null && bossHpFill == null) return;
        if (_bossMax <= 0) return;

        float ratio = Mathf.Clamp01(_bossCurrent / (float)_bossMax);

        if (bossHpFill != null)
        {
            _bossFillShown = Mathf.Lerp(_bossFillShown, ratio, 1f - Mathf.Exp(-bossFillLerpSpeed * Time.deltaTime));
            bossHpFill.fillAmount = _bossFillShown;

            // restore base color if changed elsewhere
            bossHpFill.color = _bossFillBaseColor;
        }

        // Auto-show boss panel if receiving boss HP updates
        if (bossPanelRoot != null && !bossPanelRoot.activeSelf)
            bossPanelRoot.SetActive(true);
    }

    private void CachePlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerHp = p.GetComponent<PlayerHealth>();
        if (_playerHp == null) _playerHp = FindFirstObjectByType<PlayerHealth>();
    }

    // =========================
    // Public API (giữ tương thích)
    // =========================

    public void SetWave(int current, int total)
    {
        if (waveText != null) waveText.text = $"Wave: {current}/{total}";
    }

    public void SetWaveText(string text)
    {
        if (waveText != null) waveText.text = text;
    }

    public void SetTimerSeconds(float seconds)
    {
        if (timerText != null) timerText.text = $"Time: {FormatTime(seconds)}";
    }

    public void SetBossHP(int current, int max)
    {
        _bossCurrent = Mathf.Max(0, current);
        _bossMax = Mathf.Max(1, max);

        if (bossHpText != null)
            bossHpText.text = $"Boss: {_bossCurrent}/{_bossMax}";

        if (bossPanelRoot != null && !bossPanelRoot.activeSelf)
            bossPanelRoot.SetActive(true);
    }

    public void ClearBossHP()
    {
        _bossCurrent = 0;
        _bossMax = 0;

        if (bossHpText != null) bossHpText.text = "";
        if (bossHpFill != null) bossHpFill.fillAmount = 0f;

        if (bossPanelRoot != null)
            bossPanelRoot.SetActive(false);
    }

    public void SetBossPanelVisible(bool visible)
    {
        if (bossPanelRoot != null) bossPanelRoot.SetActive(visible);
    }

    public void ShowWarning(string msg, float seconds)
    {
        if (warningText == null) return;

        if (_warningCo != null) StopCoroutine(_warningCo);
        _warningCo = StartCoroutine(WarningRoutine(msg, seconds));
    }

    private IEnumerator WarningRoutine(string msg, float seconds)
    {
        warningText.gameObject.SetActive(true);
        warningText.text = msg;

        float t = 0f;
        while (t < seconds)
        {
            if (StageResultUI.IsShowingResult) yield break; // win/lose rồi thì khỏi show
            t += Time.deltaTime; // pause => deltaTime = 0 (đúng mong muốn)
            yield return null;
        }

        warningText.text = "";
        warningText.gameObject.SetActive(false);
        _warningCo = null;
    }

    // =========================
    // Damage flash
    // =========================

    public void FlashDamage(float alpha = -1f, float fadeOut = -1f)
    {
        if (damageFlashImage == null) return;

        float a = (alpha < 0f) ? damageFlashDefaultAlpha : Mathf.Clamp01(alpha);
        float f = (fadeOut < 0f) ? damageFlashFadeOut : Mathf.Max(0.01f, fadeOut);

        if (_flashCo != null) StopCoroutine(_flashCo);
        _flashCo = StartCoroutine(FlashRoutine(a, f));
    }

    private IEnumerator FlashRoutine(float alpha, float fadeOut)
    {
        // snap to alpha
        var c = damageFlashImage.color;
        c.a = alpha;
        damageFlashImage.color = c;

        float t = 0f;
        while (t < fadeOut)
        {
            float dt = flashUsesUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;

            float k = 1f - Mathf.Clamp01(t / fadeOut);
            c.a = alpha * k;
            damageFlashImage.color = c;
            yield return null;
        }

        c.a = 0f;
        damageFlashImage.color = c;
        _flashCo = null;
    }

    private static string FormatTime(float seconds)
    {
        int s = Mathf.Max(0, Mathf.FloorToInt(seconds));
        int m = s / 60;
        int r = s % 60;
        return $"{m:00}:{r:00}";
    }
}