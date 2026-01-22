/*using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageResultUI : MonoBehaviour
{
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    public void ShowWin()
    {
        Time.timeScale = 0f;
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = "YOU WIN!";
    }

    public void ShowLose()
    {
        Time.timeScale = 0f;
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = "YOU LOSE!";
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.GoMenu();
        else SceneManager.LoadScene("01_Menu");
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        if (resultPanel != null) resultPanel.SetActive(false);
    }
}
*/



using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageResultUI : MonoBehaviour
{
    public static bool IsShowingResult { get; private set; }

    [Header("UI")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    [Header("Optional - link PauseUI để tắt Pause khi Win/Lose")]
    public PauseUI pauseUI;

    private void Start()
    {
        IsShowingResult = false;

        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    public void ShowWin()
    {
        ShowResult("YOU WIN!");
    }

    public void ShowLose()
    {
        ShowResult("YOU LOSE!");
    }

    private void ShowResult(string msg)
    {
        IsShowingResult = true;

        // nếu đang pause thì tắt pause panel để không bị chồng
        if (pauseUI != null)
            pauseUI.ForceClose();

        Time.timeScale = 0f;

        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = msg;
    }

    public void BackToMenu()
    {
        IsShowingResult = false;
        Time.timeScale = 1f;

        if (GameManager.Instance != null) GameManager.Instance.GoMenu();
        else SceneManager.LoadScene("01_Menu");
    }

    public void RestartStage()
    {
        IsShowingResult = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Hide()
    {
        IsShowingResult = false;
        Time.timeScale = 1f;

        if (resultPanel != null) resultPanel.SetActive(false);
    }
}
