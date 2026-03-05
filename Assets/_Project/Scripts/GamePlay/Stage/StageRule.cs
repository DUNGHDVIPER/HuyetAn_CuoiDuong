using UnityEngine;

public class StageRule : MonoBehaviour
{
    [Header("Refs")]
    public StageResultUI resultUI;

    [Header("Win/Lose Handler (optional)")]
    public StageWinLoseHandler winLoseHandler;

    [Header("Win Zone - unlock after boss dies")]
    public GameObject winZone; // kéo WinZone vào đây

    private bool ended = false;

    private void Awake()
    {
        if (resultUI == null)
            resultUI = FindFirstObjectByType<StageResultUI>();

        if (winLoseHandler == null)
            winLoseHandler = FindFirstObjectByType<StageWinLoseHandler>();

        // nếu có winZone thì đảm bảo ban đầu ẩn
        if (winZone != null)
            winZone.SetActive(false);
    }

    // gọi khi boss chết -> mở cổng thắng (cũ)
    public void UnlockWinZone()
    {
        if (ended) return;

        if (winZone != null)
        {
            winZone.SetActive(true);
            Debug.Log("[StageRule] WinZone unlocked!");
        }
        else
        {
            Debug.LogWarning("[StageRule] winZone is NULL - assign it in Inspector!");
        }
    }

    // gọi khi player chạm winzone -> WIN thật
    public void TriggerWin(string reason = "Win")
    {
        if (ended) return;
        ended = true;

        Debug.Log($"[StageRule] WIN: {reason}");

        // 1) giữ flow cũ
        if (resultUI != null) resultUI.ShowWin();

        // 2) thêm flow mới (portal đi tiếp)
        winLoseHandler?.OnWin();
    }

    public void TriggerLose(string reason = "Lose")
    {
        if (ended) return;
        ended = true;

        Debug.Log($"[StageRule] LOSE: {reason}");

        // gọi handler trước (để bật LosePanel)
        winLoseHandler?.OnLose();

        // giữ flow cũ
        if (resultUI != null) resultUI.ShowLose();
    }
}