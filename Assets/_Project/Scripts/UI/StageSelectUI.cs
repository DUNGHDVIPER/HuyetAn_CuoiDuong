using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StageSelectUI : MonoBehaviour
{
    [Header("Buttons (size = number of stages)")]
    public Button[] stageButtons;          // kéo 5 button vào đây
    public TMP_Text[] stageLabels;         // kéo 5 TMP text vào đây (nếu bạn muốn đổi màu/đánh dấu)

    [Header("Scene names for stages (same order)")]
    public string[] stageScenes;           // ví dụ: 03_Stage1, 04_Stage2...

    [Header("Back")]
    public string menuSceneName = "01_Menu";

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int unlocked = StageProgress.UnlockedStage;
        int cleared = StageProgress.ClearedStage;

        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageIndex = i + 1; // màn bắt đầu từ 1

            bool isUnlocked = stageIndex <= unlocked;
            stageButtons[i].interactable = isUnlocked;

            // Label: hiện ✓ nếu đã qua
            if (stageLabels != null && i < stageLabels.Length && stageLabels[i] != null)
            {
                if (stageIndex <= cleared)
                {
                    stageLabels[i].text = $"Màn {stageIndex}  ✓";
                    stageLabels[i].color = Color.white;
                }
                else if (!isUnlocked)
                {
                    stageLabels[i].text = $"Màn {stageIndex}  🔒";
                    stageLabels[i].color = new Color(1f, 1f, 1f, 0.45f);
                }
                else
                {
                    stageLabels[i].text = $"Màn {stageIndex}";
                    stageLabels[i].color = Color.white;
                }
            }

            // đảm bảo click đúng stage
            int idx = i;
            stageButtons[i].onClick.RemoveAllListeners();
            stageButtons[i].onClick.AddListener(() => LoadStage(idx));
        }
    }

    public void LoadStage(int i)
    {
        if (stageScenes == null || i >= stageScenes.Length) return;
        SceneManager.LoadScene(stageScenes[i]);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    // để test nhanh trong Editor
    public void ResetProgress()
    {
        StageProgress.ResetAll();
        Refresh();
    }
}
