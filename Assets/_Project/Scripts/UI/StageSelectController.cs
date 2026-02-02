using TMPro;
using UnityEngine;
using UnityEngine.UI;
// nếu bạn dùng TMP thì dùng:
// using TMPro;

public class StageSelectController : MonoBehaviour
{
    /*public Text titleText;*/
    public TextMeshProUGUI titleText;
    // TMP: public TMP_Text titleText;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL. Hãy Play từ 00_Boot.");
            return;
        }

        int ch = GameManager.Instance.currentChapter;
        if (titleText != null)
            titleText.text = $"Chapter {ch} - Select Stage";
    }


    // stageIndex: 1,2,3 hoặc 99 cho Boss
    /* public void PlayStage(int stageIndex)
     {
         int ch = GameManager.Instance.currentChapter;

         // Quy ước tên scene stage:
         // CH1_S1, CH1_S2, CH1_S3, CH1_BOSS ...
         string stageSceneName =
             stageIndex == 99 ? $"CH{ch}_BOSS" : $"CH{ch}_S{stageIndex}";

         // chưa có stage thật cũng không sao, test bằng "Main"
         // stageSceneName = "Main";

         GameManager.Instance.LoadStage(stageSceneName);
     }
 */

    public void PlayStage(int stageIndex)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager NULL – Play từ 00_Boot!");
            return;
        }

        int ch = GameManager.Instance.currentChapter;
        string stageSceneName =
            stageIndex == 99 ? $"CH{ch}_BOSS" : $"CH{ch}_S{stageIndex}";

        Debug.Log($"[StageSelect] Load stage: {stageSceneName}");
        GameManager.Instance.LoadStage(stageSceneName);
    }

    public void BackToChapterMap()
    {
        GameManager.Instance.GoChapterMap();
    }
}
