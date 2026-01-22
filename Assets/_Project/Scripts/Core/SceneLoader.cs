using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Scene Names (must match exactly)")]
    public string bootScene = "00_Boot";
    public string menuScene = "01_Menu";
    public string loadingScene = "99_Loading";

    private string _targetScene;

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

    /// <summary>Go to Menu directly (no loading)</summary>
    public void GoToMenu()
    {
        GameManager.Instance.SetState(GameState.Menu);
        SceneManager.LoadScene(menuScene);
    }

    /// <summary>Load a scene using 99_Loading as intermediate</summary>
    public void LoadWithLoading(string targetScene)
    {
        _targetScene = targetScene;
        GameManager.Instance.SetState(GameState.Loading);
        SceneManager.LoadScene(loadingScene);
    }

    // This will be called from Loading scene (script)
    public void StartLoadingTarget()
    {
        StartCoroutine(LoadTargetRoutine());
    }

    private IEnumerator LoadTargetRoutine()
    {
        // wait 1 frame so Loading scene is fully shown
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(_targetScene);
        while (!op.isDone)
        {
            yield return null;
        }

        GameManager.Instance.SetState(GameState.Playing);
    }
}
