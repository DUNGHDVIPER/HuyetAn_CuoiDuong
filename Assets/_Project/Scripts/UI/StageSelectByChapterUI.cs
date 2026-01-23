using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StageSelectByChapterUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text title;
    public Button btnStage1;
    public Button btnStage2;
    public Button btnStage3;
    public Button btnBoss;
    public Button btnBack;

    [Header("Scenes - set in Inspector")]
    public string chapterMapScene = "02_ChapterMap";

    // Mỗi chương có thể có 1..4 scene: Stage1, Stage2, Stage3, Boss (nếu có)
    public string[] chapter1Scenes;
    public string[] chapter2Scenes;
    public string[] chapter3Scenes;
    public string[] chapter4Scenes;
    public string[] chapter5Scenes;

    private const string KEY_SELECTED_CHAPTER = "SELECTED_CHAPTER";
    private const string KEY_UNLOCKED_STAGE_PREFIX = "UNLOCKED_STAGE_CH"; // VD: UNLOCKED_STAGE_CH5 = 1..4

    void Start()
    {
        int chapter = PlayerPrefs.GetInt(KEY_SELECTED_CHAPTER, 1);

        // Title
        if (title != null) title.text = $"Chương {chapter} - Chọn Màn";

        // Lấy danh sách scene của chương
        var scenes = GetScenesByChapter(chapter);

        // Nếu chưa set gì -> báo rõ
        if (scenes == null || scenes.Length == 0)
        {
            Debug.LogError($"[StageSelectByChapterUI] Chưa khai báo scene cho Chương {chapter}. " +
                           $"Hãy nhập ít nhất 1 scene vào Chapter {chapter} Scenes trong Inspector.");
            DisableAllStageButtons();
            BindBack();
            return;
        }

        // Số stage mở khóa (mặc định: mở stage 1)
        int unlockedStage = PlayerPrefs.GetInt(KEY_UNLOCKED_STAGE_PREFIX + chapter, 1);

        // Gán từng nút theo số scene thật sự đang có
        SetupStageButton(btnStage1, scenes, 0, unlockedStage >= 1);
        SetupStageButton(btnStage2, scenes, 1, unlockedStage >= 2);
        SetupStageButton(btnStage3, scenes, 2, unlockedStage >= 3);
        SetupStageButton(btnBoss, scenes, 3, unlockedStage >= 4);

        BindBack();
    }

    void SetupStageButton(Button btn, string[] scenes, int index, bool unlocked)
    {
        if (btn == null) return;

        bool hasScene = scenes != null && index < scenes.Length && !string.IsNullOrEmpty(scenes[index]);

        btn.onClick.RemoveAllListeners();

        // Nếu không có scene ở slot đó -> khóa luôn (dù unlockedStage có cao)
        btn.interactable = hasScene && unlocked;

        if (btn.interactable)
        {
            string sceneName = scenes[index];
            btn.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
        }
    }

    void DisableAllStageButtons()
    {
        if (btnStage1 != null) btnStage1.interactable = false;
        if (btnStage2 != null) btnStage2.interactable = false;
        if (btnStage3 != null) btnStage3.interactable = false;
        if (btnBoss != null) btnBoss.interactable = false;
    }

    void BindBack()
    {
        if (btnBack == null) return;
        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(() => SceneManager.LoadScene(chapterMapScene));
    }

    string[] GetScenesByChapter(int chapter)
    {
        return chapter switch
        {
            1 => chapter1Scenes,
            2 => chapter2Scenes,
            3 => chapter3Scenes,
            4 => chapter4Scenes,
            5 => chapter5Scenes,
            _ => chapter1Scenes
        };
    }

    // Optional: gọi bằng button để test
    public void ForceChapter(int chapter)
    {
        PlayerPrefs.SetInt(KEY_SELECTED_CHAPTER, chapter);
        PlayerPrefs.Save();
        Debug.Log("[StageSelectByChapterUI] Force chapter = " + chapter);
    }
}
