/*using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StageSelectByChapterUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text title;         // "Chương 5 - Kẻ Thù Cuối"
    public TMP_Text subTitle;      // optional: "Chọn Huyết Ấn"
    public Button btnStage1;
    public Button btnStage2;
    public Button btnStage3;
    public Button btnBoss;
    public Button btnBack;

    [Header("Scenes")]
    public string chapterMapScene = "02_ChapterMap";

    // Mỗi chương: 4 scene theo thứ tự: S1, S2, S3, BOSS
    public string[] chapter1Scenes = new string[4];
    public string[] chapter2Scenes = new string[4];
    public string[] chapter3Scenes = new string[4];
    public string[] chapter4Scenes = new string[4];
    public string[] chapter5Scenes = new string[4];

    [Header("Chapter Titles (hiện tiêu đề chương)")]
    public string[] chapterTitles = new string[5] {
        "Chương 1 - Khởi Hành",
        "Chương 2 - Huyết Ấn Trỗi Dậy",
        "Chương 3 - Con Đường Đen",
        "Chương 4 - Lời Thề Trả Thù",
        "Chương 5 - Kẻ Thù Cuối"
    };

    private const string KEY_SELECTED_CHAPTER = "SELECTED_CHAPTER";
    private const string KEY_UNLOCKED_STAGE_PREFIX = "UNLOCKED_STAGE_CH"; // + chapter => 1..4

    void Start()
    {
        int chapter = PlayerPrefs.GetInt(KEY_SELECTED_CHAPTER, 1);

        // ✅ set tiêu đề chương
        if (title != null)
        {
            string t = (chapter >= 1 && chapter <= chapterTitles.Length) ? chapterTitles[chapter - 1] : $"Chương {chapter}";
            title.text = t;
        }
        if (subTitle != null) subTitle.text = "Chọn Huyết Ấn";

        // ✅ lấy scene list theo chương
        var scenes = GetScenesByChapter(chapter);
        if (scenes == null || scenes.Length < 4 || string.IsNullOrEmpty(scenes[0]))
        {
            Debug.LogError($"[StageSelect] Chưa set đủ scene cho chương {chapter}. Vào Inspector điền 4 scene (S1,S2,S3,Boss).");
            return;
        }

        // ✅ khóa theo tiến độ: 1..4
        int unlockedStage = PlayerPrefs.GetInt(KEY_UNLOCKED_STAGE_PREFIX + chapter, 1);

        SetupStageButton(btnStage1, "Màn 1", unlockedStage >= 1, scenes[0], isBoss: false);
        SetupStageButton(btnStage2, "Màn 2", unlockedStage >= 2, scenes[1], isBoss: false);
        SetupStageButton(btnStage3, "Màn 3", unlockedStage >= 3, scenes[2], isBoss: false);
        SetupStageButton(btnBoss, "BOSS", unlockedStage >= 4, scenes[3], isBoss: true);

        if (btnBack != null)
        {
            btnBack.onClick.RemoveAllListeners();
            btnBack.onClick.AddListener(() => SceneManager.LoadScene(chapterMapScene));
        }
    }

    void SetupStageButton(Button btn, string label, bool interactable, string sceneName, bool isBoss)
    {
        if (!btn) return;

        btn.interactable = interactable;

        // ✅ đổi chữ trên nút (tự tìm TMP con)
        var tmp = btn.GetComponentInChildren<TMP_Text>(true);
        if (tmp) tmp.text = label;

        // ✅ nếu bạn dùng prefab có StageButtonUI (giống ChapterButtonUI) thì set state
        var fx = btn.GetComponent<StageButtonUI>();
        if (fx != null)
        {
            fx.SetText(label);
            if (!interactable) fx.SetState(StageButtonUI.State.Locked);
            else if (isBoss) fx.SetState(StageButtonUI.State.Boss);
            else fx.SetState(StageButtonUI.State.Unlocked);
        }

        btn.onClick.RemoveAllListeners();
        if (interactable)
        {
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"[StageSelect] Load stage: {sceneName}");
                SceneManager.LoadScene(sceneName);
            });
        }
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

    // DEV: mở full stage của chương đang chọn
    public void DevUnlockAllStagesThisChapter()
    {
        int chapter = PlayerPrefs.GetInt(KEY_SELECTED_CHAPTER, 1);
        PlayerPrefs.SetInt(KEY_UNLOCKED_STAGE_PREFIX + chapter, 4);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
*/
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using HACD; // nếu bạn dùng AppScenes

public class StageSelectByChapterUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text title;
    public TMP_Text subTitle;
    public Button btnStage1, btnStage2, btnStage3, btnBoss, btnBack;

    [Header("Scenes")]
    public string chapterMapScene = AppScenes.ChapterMap; // hoặc "02_ChapterMap"

    // Mỗi chương: 4 scene theo thứ tự: S1, S2, S3, BOSS
    public string[] chapter1Scenes = new string[4];
    public string[] chapter2Scenes = new string[4];
    public string[] chapter3Scenes = new string[4];
    public string[] chapter4Scenes = new string[4];
    public string[] chapter5Scenes = new string[4];

    [Header("Chapter Titles")]
    public string[] chapterTitles = new string[5] {
        "Chương 1 - Khởi Hành",
        "Chương 2 - Huyết Ấn Trỗi Dậy",
        "Chương 3 - Con Đường Đen",
        "Chương 4 - Lời Thề Trả Thù",
        "Chương 5 - Kẻ Thù Cuối"
    };

    private const string KEY_SELECTED_CHAPTER = "SELECTED_CHAPTER";
    private const string KEY_UNLOCKED_STAGE_PREFIX = "UNLOCKED_STAGE_CH"; // + chapter => 1..4

    void Awake()
    {
        // LOG NGAY TỪ AWAKE để bắt “đứa nào ghi đè”
        Debug.Log($"[StageSelect][Awake] SELECTED_CHAPTER = {PlayerPrefs.GetInt(KEY_SELECTED_CHAPTER, -999)}");
    }

    void Start()
    {
        int chapter = PlayerPrefs.GetInt(KEY_SELECTED_CHAPTER, 1);
        Debug.Log($"[StageSelect][Start] SELECTED_CHAPTER = {chapter}");

        // set tiêu đề
        if (title != null)
        {
            string t = (chapter >= 1 && chapter <= chapterTitles.Length)
                ? chapterTitles[chapter - 1]
                : $"Chương {chapter}";
            title.text = t;
        }
        if (subTitle != null) subTitle.text = "Chọn Huyết Ấn";

        // lấy list scene theo chương
        var scenes = GetScenesByChapter(chapter);

        // validate đủ 4 scene
        if (!IsValid4Scenes(scenes))
        {
            Debug.LogError($"[StageSelect] Chưa set đủ 4 scene cho CH{chapter} (S1,S2,S3,Boss).");
            return;
        }

        // unlock theo tiến độ
        int unlockedStage = PlayerPrefs.GetInt(KEY_UNLOCKED_STAGE_PREFIX + chapter, 1);

        Setup(btnStage1, "Huyết Ấn I", unlockedStage >= 1, scenes[0], false);
        Setup(btnStage2, "Huyết Ấn II", unlockedStage >= 2, scenes[1], false);
        Setup(btnStage3, "Huyết Ấn III", unlockedStage >= 3, scenes[2], false);
        Setup(btnBoss, "BOSS", unlockedStage >= 4, scenes[3], true);

        if (btnBack != null)
        {
            btnBack.onClick.RemoveAllListeners();
            btnBack.onClick.AddListener(() => SceneManager.LoadScene(chapterMapScene));
        }
    }

    void Setup(Button btn, string label, bool interactable, string sceneName, bool isBoss)
    {
        if (!btn) return;

        btn.interactable = interactable;

        // đổi text trên nút
        var tmp = btn.GetComponentInChildren<TMP_Text>(true);
        if (tmp) tmp.text = label;

        // nếu có StageButtonUI thì set state
        var fx = btn.GetComponent<StageButtonUI>();
        if (fx != null)
        {
            fx.SetText(label);
            if (!interactable) fx.SetState(StageButtonUI.State.Locked);
            else if (isBoss) fx.SetState(StageButtonUI.State.Boss);
            else fx.SetState(StageButtonUI.State.Unlocked);
        }

        btn.onClick.RemoveAllListeners();
        if (interactable)
        {
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"[StageSelect] Click -> Load '{sceneName}'");
                SceneManager.LoadScene(sceneName);
            });
        }
    }

    bool IsValid4Scenes(string[] scenes)
    {
        return scenes != null && scenes.Length >= 4
            && !string.IsNullOrEmpty(scenes[0])
            && !string.IsNullOrEmpty(scenes[1])
            && !string.IsNullOrEmpty(scenes[2])
            && !string.IsNullOrEmpty(scenes[3]);
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
}
