using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string chapterMapScene = "02_ChapterMap";
    public string stageSelectScene = "03_StageSelect";
    public string gameplayScene = "Main";

    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    public void OnStart()
    {
        // Flow: Start -> Select Slot -> Chapter Map -> Stage Select
        // (slot sẽ làm ở bước SaveManager sau, giờ đi thẳng ChapterMap)
        SceneLoader.Instance.LoadWithLoading(chapterMapScene);
    }

    public void OnContinue()
    {
        // Tạm: Continue -> load gameplayScene
        // Sau này đổi thành load slot + scene đã lưu
        SceneLoader.Instance.LoadWithLoading(gameplayScene);
    }

    public void OnSelectChapter()
    {
        SceneLoader.Instance.LoadWithLoading(chapterMapScene);
    }

    public void OnOpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    public void OnOpenCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    public void OnBackToMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    public void OnQuit()
    {
        Application.Quit();
        Debug.Log("Quit Game (Editor will not close)");
    }
}
