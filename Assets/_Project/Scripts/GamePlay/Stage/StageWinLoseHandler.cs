using UnityEngine;

public class StageWinLoseHandler : MonoBehaviour
{
    [Header("Win")]
    public GameObject exitPortalPrefab;     // PF_ExitPortal
    public string nextSceneName = "CH5_S2";
    public Vector3 winPortalOffset = new Vector3(3f, 0f, 0f);

    [Header("Win - Unfreeze / Re-enable Player")]
    public bool forceUnfreezeOnWin = true;
    public bool reEnablePlayerMovementOnWin = true;

    [Header("Lose")]
    public GameObject losePanel;            // Nếu quên kéo, code sẽ tự tìm trong StageHUD

    private bool _spawnedWinPortal;

    public void OnWin()
    {
        if (_spawnedWinPortal) return;
        _spawnedWinPortal = true;

        if (forceUnfreezeOnWin)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (reEnablePlayerMovementOnWin && player != null)
        {
            // Bật lại movement script (tên đúng của bạn: PlayerController2D)
            var pc = player.GetComponent<PlayerController2D>();
            if (pc != null) pc.enabled = true;

            // nếu game có script input khác bị tắt, bật lại luôn:
            foreach (var mb in player.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (mb == null) continue;
                string n = mb.GetType().Name.ToLower();
                if (n.Contains("controller") || n.Contains("movement") || n.Contains("input"))
                    mb.enabled = true;
            }
        }

        Vector3 pos = player ? player.transform.position + winPortalOffset : Vector3.zero;

        if (exitPortalPrefab != null)
        {
            var go = Instantiate(exitPortalPrefab, pos, Quaternion.identity);
            var trig = go.GetComponent<ExitPortalTrigger>();
            if (trig != null)
            {
                trig.nextSceneName = nextSceneName;
                // bạn muốn "đi vào portal" => KHÔNG auto load
                // (auto load chỉ cần nếu bạn bật option trong ExitPortalTrigger)
            }
        }
        else
        {
            Debug.LogWarning("[StageWinLoseHandler] exitPortalPrefab missing");
        }
    }

    public void OnLose()
    {
        // 1) cố lấy đúng LosePanel nếu bạn quên kéo
        if (losePanel == null)
            losePanel = FindLosePanelInHUD();

        if (losePanel == null)
        {
            Debug.LogWarning("[StageWinLoseHandler] LosePanel not found. Drag it in Inspector or ensure it's under PF_StageHUD/SafeArea.");
            return;
        }

        // 2) bật panel
        losePanel.SetActive(true);

        // 3) đưa lên top UI để không bị fog/overlay che
        losePanel.transform.SetAsLastSibling();

        // 4) (tuỳ bạn) dừng gameplay để khỏi bị đánh tiếp
        Time.timeScale = 0f;
    }

    private GameObject FindLosePanelInHUD()
    {
        // Ưu tiên HUD chính
        StageHUD hud = StageHUD.Main != null ? StageHUD.Main : FindFirstObjectByType<StageHUD>();
        if (hud != null)
        {
            // tìm theo tên trong hierarchy: SafeArea/LosePanel
            var t = hud.transform.Find("SafeArea/LosePanel");
            if (t != null) return t.gameObject;

            // fallback: tìm sâu theo tên
            foreach (var tr in hud.GetComponentsInChildren<Transform>(true))
            {
                if (tr.name == "LosePanel") return tr.gameObject;
            }
        }

        // fallback cuối: tìm toàn scene
        var any = GameObject.Find("LosePanel");
        return any;
    }
}