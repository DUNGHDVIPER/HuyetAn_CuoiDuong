using UnityEngine;
using UnityEngine.SceneManagement;
using HACD;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    public AppGameState CurrentState = AppGameState.Boot;

    [Header("Save Slot")]
    public int currentSlot = 1;

    [Header("Progress")]
    public int currentChapter = 1;

    // stage scene sẽ load sau Loading
    public string currentStageScene = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(AppGameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameManager] State -> {newState}");
    }

    // Điều hướng scene theo AppScenes
    public void GoChapterMap() => SceneManager.LoadScene(AppScenes.ChapterMap);
    public void GoStageSelect() => SceneManager.LoadScene(AppScenes.StageSelect);
    public void GoBoot() => SceneManager.LoadScene(AppScenes.Boot);
    public void GoMenu() => SceneManager.LoadScene(AppScenes.Menu);

    /// <summary>
    /// StageSelect gọi hàm này -> set stage -> vào Loading
    /// </summary>
    public void LoadStage(string stageSceneName)
    {
        currentStageScene = stageSceneName;
        SetState(AppGameState.Loading);
        SceneManager.LoadScene(AppScenes.Loading);
    }

    public void SetStage(string sceneName)
    {
        currentStageScene = sceneName;
        Debug.Log($"[GameManager] SetStage = {currentStageScene}");
    }
}
