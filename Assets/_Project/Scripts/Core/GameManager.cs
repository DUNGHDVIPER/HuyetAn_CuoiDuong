using UnityEngine;

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
}
