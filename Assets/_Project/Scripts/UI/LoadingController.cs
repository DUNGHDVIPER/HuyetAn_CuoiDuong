using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    [Header("Optional")]
    public float minShowTime = 0.5f;

    [Header("Fallback")]
    public string fallbackSceneIfMissing = "00_Boot";   // quay về Boot nếu thiếu GameManager
    public string fallbackStageIfEmpty = "Main";        // nếu currentStageScene rỗng thì load Main

    private IEnumerator Start()
    {
        // Đợi 1 frame để chắc chắn GameManager đã kịp Awake (nếu vừa chuyển scene)
        yield return null;

        // 1) đảm bảo có GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found! Bạn đang Play sai scene. Tự quay về 00_Boot.");

            // KHÔNG yield break (sẽ kẹt). Quay về Boot luôn.
            SceneManager.LoadScene(fallbackSceneIfMissing);
            yield break;
        }

        // 2) lấy scene cần load
        string target = GameManager.Instance.currentStageScene;
        if (string.IsNullOrEmpty(target))
        {
            Debug.LogWarning("currentStageScene rỗng -> fallback Main");
            target = fallbackStageIfEmpty;
        }

        // 3) load async
        float startTime = Time.time;
        AsyncOperation op = SceneManager.LoadSceneAsync(target);

        if (op == null)
        {
            Debug.LogError($"LoadSceneAsync failed for '{target}'. Kiểm tra scene có nằm trong Build Settings không!");
            SceneManager.LoadScene(fallbackSceneIfMissing);
            yield break;
        }

        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            // op.progress lên tới 0.9 là load gần xong
            if (op.progress >= 0.9f)
            {
                // giữ loading tối thiểu minShowTime
                if (Time.time - startTime >= minShowTime)
                    op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
