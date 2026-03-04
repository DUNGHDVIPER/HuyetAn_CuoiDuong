using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageHUD_CH3 : MonoBehaviour
{
    public static StageHUD_CH3 Main { get; private set; }

    [Header("Title")]
    public TextMeshProUGUI title;

    [Header("Texts")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI waveText;   // sẽ dùng làm Kill Text
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bossHpText;
    public TextMeshProUGUI warningText;

    [Header("Bars")]
    public Image playerHpFill;
    public Image bossHpFill;
    public GameObject bossPanelRoot;

    [Header("Stage Settings")]
    public bool autoTimer = true;
    public float stageDuration = 45f;

    [Header("Smooth Settings")]
    public float hpLerpSpeed = 10f;
    public float bossLerpSpeed = 10f;

    private PlayerHealth1_CH3 _player;

    private float _timer = 0f;
    private int _killCount = 0;

    private float _playerFillShown = 1f;
    private float _bossFillShown = 1f;

    private int _bossCurrent;
    private int _bossMax;

    // =====================================================

    private void Awake()
    {
        Main = this;
    }

    private void Start()
    {
        SetupTitle();
        CachePlayer();

        if (bossPanelRoot != null)
            bossPanelRoot.SetActive(false);

        _timer = 0f;
        UpdateKillUI();
        SetTimerSeconds(_timer);
    }

    private void Update()
    {
        UpdatePlayerHP();
        UpdateBossHP();

        if (autoTimer)
            UpdateTimerUp();
    }

    // =====================================================
    // TITLE
    // =====================================================

    private void SetupTitle()
    {
        if (title == null) return;

        string scene = SceneManager.GetActiveScene().name;

        if (scene.StartsWith("CH"))
        {
            int us = scene.IndexOf('_');
            if (us > 2)
            {
                string ch = scene.Substring(2, us - 2);

                if (scene.Contains("_S"))
                {
                    int s = scene.IndexOf("_S");
                    string st = scene.Substring(s + 2);
                    title.text = $"Chapter {ch} - Stage {st}";
                }
                else if (scene.Contains("_B"))
                {
                    title.text = $"Chapter {ch} - BOSS";
                }
            }
        }
        else
        {
            title.text = scene;
        }
    }

    // =====================================================
    // TIMER ĐẾM LÊN
    // =====================================================

    private void UpdateTimerUp()
    {
        if (_timer >= stageDuration) return;

        _timer += Time.deltaTime;
        SetTimerSeconds(_timer);
    }

    public void SetTimerSeconds(float seconds)
    {
        if (timerText == null) return;
        timerText.text = "Time: " + FormatTime(seconds);
    }

    private string FormatTime(float seconds)
    {
        int s = Mathf.Max(0, Mathf.FloorToInt(seconds));
        int m = s / 60;
        int r = s % 60;
        return $"{m:00}:{r:00}";
    }

    // =====================================================
    // KILL COUNT
    // =====================================================

    public void AddKill()
    {
        _killCount++;
        UpdateKillUI();
    }

    private void UpdateKillUI()
    {
        if (waveText != null)
            waveText.text = $"Kills: {_killCount}";
    }

    // =====================================================
    // PLAYER HP
    // =====================================================

    private void CachePlayer()
    {
        var obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            _player = obj.GetComponent<PlayerHealth1_CH3>();

        if (_player == null)
            _player = FindFirstObjectByType<PlayerHealth1_CH3>();
    }

    private void UpdatePlayerHP()
    {
        if (_player == null)
        {
            CachePlayer();
            if (_player == null) return;
        }

        int cur = _player.CurrentHP;
        int max = Mathf.Max(1, _player.maxHP);

        float ratio = Mathf.Clamp01(cur / (float)max);

        if (hpText != null)
            hpText.text = $"HP: {cur}/{max}";

        if (playerHpFill != null)
        {
            _playerFillShown = Mathf.Lerp(_playerFillShown, ratio, hpLerpSpeed * Time.deltaTime);
            playerHpFill.fillAmount = _playerFillShown;
        }
    }

    // =====================================================
    // BOSS HP
    // =====================================================

    private void UpdateBossHP()
    {
        if (_bossMax <= 0) return;

        float ratio = Mathf.Clamp01(_bossCurrent / (float)_bossMax);

        if (bossHpFill != null)
        {
            _bossFillShown = Mathf.Lerp(_bossFillShown, ratio, bossLerpSpeed * Time.deltaTime);
            bossHpFill.fillAmount = _bossFillShown;
        }

        if (bossHpText != null)
            bossHpText.text = $"Boss: {_bossCurrent}/{_bossMax}";
    }

    public void SetBossHP(int current, int max)
    {
        _bossCurrent = current;
        _bossMax = Mathf.Max(1, max);

        if (bossPanelRoot != null)
            bossPanelRoot.SetActive(true);
    }

    public void ClearBossHP()
    {
        _bossCurrent = 0;
        _bossMax = 0;

        if (bossPanelRoot != null)
            bossPanelRoot.SetActive(false);
    }

    // =====================================================
    // WARNING
    // =====================================================

    public void ShowWarning(string msg, float duration)
    {
        if (warningText == null) return;
        StartCoroutine(WarningRoutine(msg, duration));
    }

    private IEnumerator WarningRoutine(string msg, float duration)
    {
        warningText.gameObject.SetActive(true);
        warningText.text = msg;

        yield return new WaitForSeconds(duration);

        warningText.gameObject.SetActive(false);
    }
}