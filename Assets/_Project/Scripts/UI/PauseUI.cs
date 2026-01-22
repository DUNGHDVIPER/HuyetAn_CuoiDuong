using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;

    private bool isPaused = false;

    /* private void Start()
     {
         Resume(); // đảm bảo vào scene là chạy bình thường
     }*/

    private void Start()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }


    public void TogglePause()
    {
        Debug.Log("[PauseUI] TogglePause clicked!");
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.GoMenu();
        else SceneManager.LoadScene("01_Menu");
    }
}
