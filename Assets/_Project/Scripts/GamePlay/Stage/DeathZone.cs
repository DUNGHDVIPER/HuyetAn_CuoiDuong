using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public string playerTag = "Player";
    public StageRule stageRule;

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
            stageRule.TriggerLose("Fell into DeathZone");
        }
    }
}
