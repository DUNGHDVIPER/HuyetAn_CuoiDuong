/*using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ChapterSelectUI : MonoBehaviour
{
    [Header("Buttons (Btn_01..Btn_05)")]
    public Button[] chapterButtons;

    [Header("Optional labels (nếu bạn dùng TMP riêng ngoài prefab)")]
    public TMP_Text[] chapterLabels;

    [Header("Flow")]
    public string stageSelectScene = "03_StageSelect";
    public string menuScene = "01_Menu";

    [Header("Unlock")]
    [Tooltip("Chương boss để hiện aura đỏ")]
    public int bossChapter = 5;

    [Tooltip("Mặc định người chơi mở được tới chương này nếu chưa có PlayerPrefs")]
    public int defaultUnlockedChapter = 1;

    private const string KEY_SELECTED_CHAPTER = "SELECTED_CHAPTER";
    private const string KEY_UNLOCKED_CHAPTER = "UNLOCKED_CHAPTER";

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (chapterButtons == null || chapterButtons.Length == 0) return;

        // ✅ CHUẨN: lấy unlock theo PlayerPrefs, nếu chưa có thì dùng defaultUnlockedChapter
        int unlockedChapter = PlayerPrefs.GetInt(KEY_UNLOCKED_CHAPTER, defaultUnlockedChapter);

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            int chapter = i + 1;                // 1..5
            int chapterLocal = chapter;         // ✅ FIX closure: giữ đúng chapter cho OnClick

            Button btn = chapterButtons[i];
            if (btn == null) continue;

            bool isUnlocked = chapter <= unlockedChapter;
            btn.interactable = isUnlocked;

            // ✅ set chữ tự động nếu bạn có mảng label ngoài prefab
            if (chapterLabels != null && i < chapterLabels.Length && chapterLabels[i] != null)
            {
                chapterLabels[i].text = $"Chương {chapter}";
                chapterLabels[i].alpha = isUnlocked ? 1f : 0.35f;
            }

            // ✅ Nếu bạn dùng prefab có ChapterButtonFX thì set label + state ngay trên prefab
            var fx = btn.GetComponent<ChapterButtonFX>();
            if (fx != null)
            {
                fx.SetText($"Chương {chapter}");
                if (!isUnlocked) fx.SetState(ChapterButtonFX.State.Locked);
                else if (chapter == bossChapter) fx.SetState(ChapterButtonFX.State.Boss);
                else fx.SetState(ChapterButtonFX.State.Unlocked);
            }

            // ✅ click: gọi thẳng chapter (không dùng idx)
            btn.onClick.RemoveAllListeners();
            if (isUnlocked)
                btn.onClick.AddListener(() => SelectChapter(chapterLocal));
        }
    }

    // ✅ FIX: nhận chapter thật (1..5), set PlayerPrefs rồi qua StageSelect
    void SelectChapter(int chapter)
    {
        PlayerPrefs.SetInt(KEY_SELECTED_CHAPTER, chapter);
        PlayerPrefs.Save();
        SceneManager.LoadScene(stageSelectScene);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    // ✅ nút test: mở tất cả chương (gọi từ Inspector)
    public void DevUnlockAll()
    {
        PlayerPrefs.SetInt(KEY_UNLOCKED_CHAPTER, 5);
        PlayerPrefs.Save();
        Refresh();
    }

    // ✅ nút test: reset về chỉ mở CH1
    public void DevResetUnlock()
    {
        PlayerPrefs.SetInt(KEY_UNLOCKED_CHAPTER, 1);
        PlayerPrefs.Save();
        Refresh();
    }
}
*/
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ChapterSelectUI : MonoBehaviour
{
    [Header("Buttons (btn_01..btn_05)")]
    public Button[] chapterButtons;

    [Header("Optional labels (nếu bạn có TMP riêng ngoài prefab)")]
    public TMP_Text[] chapterLabels;

    [Header("Flow")]
    public string stageSelectScene = "03_StageSelect";
    public string menuScene = "01_Menu";

    [Header("Unlock")]
    public int bossChapter = 5;
    public int defaultUnlockedChapter = 1;

    private const string KEY_SELECTED_CHAPTER = "SELECTED_CHAPTER";
    private const string KEY_UNLOCKED_CHAPTER = "UNLOCKED_CHAPTER";

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (chapterButtons == null || chapterButtons.Length == 0) return;

        int unlockedChapter = PlayerPrefs.GetInt(KEY_UNLOCKED_CHAPTER, defaultUnlockedChapter);

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            int chapter = i + 1;
            int chapterLocal = chapter; // ✅ chống bug closure

            Button btn = chapterButtons[i];
            if (btn == null) continue;

            bool isUnlocked = chapterLocal <= unlockedChapter;
            btn.interactable = isUnlocked;

            // nếu có label ngoài prefab
            if (chapterLabels != null && i < chapterLabels.Length && chapterLabels[i] != null)
            {
                chapterLabels[i].text = $"Chương {chapterLocal}";
                chapterLabels[i].alpha = isUnlocked ? 1f : 0.35f;
            }

            // nếu dùng FX trong prefab
            var fx = btn.GetComponent<ChapterButtonFX>();
            if (fx != null)
            {
                fx.SetText($"Chương {chapterLocal}");
                if (!isUnlocked) fx.SetState(ChapterButtonFX.State.Locked);
                else if (chapterLocal == bossChapter) fx.SetState(ChapterButtonFX.State.Boss);
                else fx.SetState(ChapterButtonFX.State.Unlocked);
            }

            // ✅ QUAN TRỌNG: không cần OnClick gán tay trong inspector
            btn.onClick.RemoveAllListeners();
            if (isUnlocked)
                btn.onClick.AddListener(() => SelectChapter(chapterLocal));
        }
    }

    void SelectChapter(int chapter)
    {
        Debug.Log($"[ChapterSelectUI] SelectChapter = {chapter}");

        PlayerPrefs.SetInt(KEY_SELECTED_CHAPTER, chapter);
        PlayerPrefs.Save();

        SceneManager.LoadScene(stageSelectScene);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    // DEV
    public void DevUnlockAll()
    {
        PlayerPrefs.SetInt(KEY_UNLOCKED_CHAPTER, 5);
        PlayerPrefs.Save();
        Refresh();
    }

    public void DevResetUnlock()
    {
        PlayerPrefs.SetInt(KEY_UNLOCKED_CHAPTER, 1);
        PlayerPrefs.Save();
        Refresh();
    }
}
