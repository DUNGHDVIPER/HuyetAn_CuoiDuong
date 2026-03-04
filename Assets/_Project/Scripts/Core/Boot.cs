using UnityEngine;

public class Boot : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found in Boot scene!");
            return;
        }
        SceneLoader.Instance.GoToMenu();
    }
}
