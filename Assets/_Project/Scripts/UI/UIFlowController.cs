using UnityEngine;

public enum UIState
{
    Playing,
    Paused,
    Result
}

public class UIFlowController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject resultPanel;

    [Header("Current")]
    public UIState state = UIState.Playing;

    private void Awake()
    {
        Apply(UIState.Playing); // vào scene là chơi
    }

    public bool CanPause()
    {
        // đã Win/Lose thì không pause nữa
        return state != UIState.Result;
    }

    public void Apply(UIState newState)
    {
        state = newState;

        // 1) bật/tắt panel đúng chuẩn
        if (hudPanel != null) hudPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(state == UIState.Paused);
        if (resultPanel != null) resultPanel.SetActive(state == UIState.Result);

        // 2) timescale theo state
        Time.timeScale = (state == UIState.Playing) ? 1f : 0f;
    }

    public void ShowPause()
    {
        if (!CanPause()) return;
        Apply(UIState.Paused);
    }

    public void HidePause()
    {
        if (state == UIState.Paused) Apply(UIState.Playing);
    }

    public void ShowResult()
    {
        // Result luôn ưu tiên cao nhất: bật result và tắt pause
        Apply(UIState.Result);
    }
}
