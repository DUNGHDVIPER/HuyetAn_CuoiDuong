using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenuUI : MonoBehaviour
{
    public string menuSceneName = "MainMenu"; // đổi đúng tên scene menu của bạn

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}