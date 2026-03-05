using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortalTrigger : MonoBehaviour
{
    public string nextSceneName;
    public float enterHoldSeconds = 0.0f;

    private float _t;
    private bool _loading;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_loading) return;
        if (!other.CompareTag("Player")) return;

        if (enterHoldSeconds <= 0f)
        {
            LoadNext();
            return;
        }

        _t += Time.deltaTime;
        if (_t >= enterHoldSeconds) LoadNext();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _t = 0f;
    }

    private void LoadNext()
    {
        if (_loading) return;
        _loading = true;

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning("[ExitPortalTrigger] nextSceneName is empty");
            _loading = false;
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}