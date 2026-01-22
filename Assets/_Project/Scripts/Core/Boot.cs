using UnityEngine;

public class Boot : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.SetState(GameState.Boot);
        SceneLoader.Instance.GoToMenu();
    }
}
