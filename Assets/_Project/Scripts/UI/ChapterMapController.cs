using UnityEngine;

public class ChapterMapController : MonoBehaviour
{
    public void SelectChapter(int chapter)
    {
        Debug.Log($"[ChapterMapController] SelectChapter({chapter})");
        Debug.Log($"[Before] currentChapter = {GameManager.Instance.currentChapter}");

        GameManager.Instance.currentChapter = chapter;
        GameManager.Instance.GoStageSelect();
    }

    public void BackToMenu()
    {
        GameManager.Instance.GoMenu();
    }
}
