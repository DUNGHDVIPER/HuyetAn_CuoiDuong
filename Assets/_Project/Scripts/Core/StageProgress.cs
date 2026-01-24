using UnityEngine;

public static class StageProgress
{
    private const string KEY_UNLOCKED = "UNLOCKED_STAGE"; // màn mở cao nhất
    private const string KEY_CLEARED = "CLEARED_STAGE";   // màn đã qua cao nhất

    public static int UnlockedStage
    {
        get => PlayerPrefs.GetInt(KEY_UNLOCKED, 1);
        set { PlayerPrefs.SetInt(KEY_UNLOCKED, Mathf.Max(1, value)); PlayerPrefs.Save(); }
    }

    public static int ClearedStage
    {
        get => PlayerPrefs.GetInt(KEY_CLEARED, 0);
        set { PlayerPrefs.SetInt(KEY_CLEARED, Mathf.Max(0, value)); PlayerPrefs.Save(); }
    }

    // gọi khi win màn i => mở màn i+1
    public static void MarkCleared(int stageIndex)
    {
        if (stageIndex > ClearedStage) ClearedStage = stageIndex;
        if (stageIndex + 1 > UnlockedStage) UnlockedStage = stageIndex + 1;
    }

    // để test reset
    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(KEY_UNLOCKED);
        PlayerPrefs.DeleteKey(KEY_CLEARED);
        PlayerPrefs.Save();
    }
}
