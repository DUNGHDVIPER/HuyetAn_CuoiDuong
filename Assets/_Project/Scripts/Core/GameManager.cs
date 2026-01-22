/*using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    public GameState CurrentState = GameState.Boot;

    [Header("Save Slot")]
    public int currentSlot = 1;

    [Header("Progress")]
    public int currentChapter = 1;
    public string currentStageScene = "Main"; // tạm thời

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameManager] State -> {newState}");
    }
    public void GoChapterMap()
    {
        SceneManager.LoadScene(SceneName.ChapterMap);
    }

    public void GoStageSelect()
    {
        SceneManager.LoadScene(SceneName.StageSelect);
    }

    public void LoadStage(string stageSceneName)
    {
        currentStageScene = stageSceneName;
        SceneManager.LoadScene(SceneName.Loading);
    }
    public void GoBoot()
    {
        SceneManager.LoadScene(SceneName.Boot);
    }

    public void GoMenu()
    {
        SceneManager.LoadScene(SceneName.Menu);
    }

    // nếu bạn đang gọi GoToMenu ở chỗ nào đó:
    public void GoToMenu()
    {
        GoMenu();
    }
}
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State")]
    public GameState CurrentState = GameState.Boot;

    [Header("Save Slot")]
    public int currentSlot = 1;

    [Header("Progress")]
    public int currentChapter = 1;

    // IMPORTANT: scene này phải tồn tại trong Build Settings nếu bạn muốn load thật
    public string currentStageScene = "Main"; // tạm test

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

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameManager] State -> {newState}");
    }

    public void GoChapterMap() => SceneManager.LoadScene(SceneName.ChapterMap);

    public void GoStageSelect() => SceneManager.LoadScene(SceneName.StageSelect);

    public void LoadStage(string stageSceneName)
    {
        currentStageScene = stageSceneName;
        SceneManager.LoadScene(SceneName.Loading);
    }

    public void GoBoot() => SceneManager.LoadScene(SceneName.Boot);

    public void GoMenu() => SceneManager.LoadScene(SceneName.Menu);

    public void GoToMenu() => GoMenu(); // alias
}
