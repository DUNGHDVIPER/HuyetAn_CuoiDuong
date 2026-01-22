using UnityEngine;

public class WinZone : MonoBehaviour
{
    public StageRule stageRule;
    public string playerTag = "Player";

    private void Awake()
    {
        if (stageRule == null)
            stageRule = FindFirstObjectByType<StageRule>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (stageRule == null) return;

        if (other.CompareTag(playerTag))
        {
            stageRule.TriggerWin("Entered WinZone");
        }
    }
}
