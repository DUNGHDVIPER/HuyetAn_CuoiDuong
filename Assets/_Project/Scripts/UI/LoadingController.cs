using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using HACD;

public class LoadingController : MonoBehaviour
{
    public float minShowTime = 0.5f;

    public string fallbackSceneIfMissing = AppScenes.Boot;
    public string fallbackReturnScene = AppScenes.StageSelect;

    private IEnumerator Start()
    {
        yield return null;

        if (GameManager.Instance == null)
        {
            Debug.LogError("[Loading] GameManager.Instance == null. Hãy Play từ 00_Boot.");
            SceneManager.LoadScene(fallbackSceneIfMissing);
            yield break;
        }

        string target = GameManager.Instance.currentStageScene;
        Debug.Log($"[Loading] currentStageScene = '{target}'");

        if (string.IsNullOrEmpty(target))
        {
            Debug.LogError("[Loading] currentStageScene rỗng -> quay về StageSelect.");
            SceneManager.LoadScene(fallbackReturnScene);
            yield break;
        }

        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            Debug.LogError($"[Loading] Scene '{target}' chưa có trong Build Settings! -> quay về StageSelect.");
            SceneManager.LoadScene(fallbackReturnScene);
            yield break;
        }

        float startTime = Time.time;
        AsyncOperation op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f && Time.time - startTime >= minShowTime)
                op.allowSceneActivation = true;

            yield return null;
        }
    }
}
