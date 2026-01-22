using TMPro;
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
