
using UnityEngine;
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

        int unlockedChapter = PlayerPrefs.GetInt(KEY_UNLOCKED_CHAPTER, defaultUnlockedChapter);
        /*int unlockedChapter = PlayerPrefs.GetInt(KEY_UNLOCKED_CHAPTER, 2);*/ // dong nay dung de test muon mo toi chuong nao thi mo



        for (int i = 0; i < chapterButtons.Length; i++)
        {
            int chapter = i + 1;
            Button btn = chapterButtons[i];
            if (btn == null) continue;

            bool isUnlocked = chapter <= unlockedChapter;
            btn.interactable = isUnlocked;

            // ✅ set chữ tự động 1..5 (2 cách: label array hoặc label trong ChapterButtonUI)
            if (chapterLabels != null && i < chapterLabels.Length && chapterLabels[i] != null)
            {
                chapterLabels[i].text = $"Chương {chapter}";
                chapterLabels[i].alpha = isUnlocked ? 1f : 0.35f;
            }

            // ✅ Nếu bạn dùng prefab có ChapterButtonUI thì set label ngay trong đó luôn
         
            var fx = btn.GetComponent<ChapterButtonFX>();
            if (fx != null)
            {
                fx.SetText($"Chương {chapter}");
                if (!isUnlocked) fx.SetState(ChapterButtonFX.State.Locked);
                else if (chapter == bossChapter) fx.SetState(ChapterButtonFX.State.Boss);
                else fx.SetState(ChapterButtonFX.State.Unlocked);
            }


            // click
            btn.onClick.RemoveAllListeners();
            int idx = i;
            if (isUnlocked)
                btn.onClick.AddListener(() => SelectChapter(idx));
        }
    }

    void SelectChapter(int index)
    {
        int chapter = index + 1;

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
