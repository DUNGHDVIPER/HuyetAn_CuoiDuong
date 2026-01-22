using UnityEngine;

public class LoadingController : MonoBehaviour
{
    private void Start()
    {
        // When 99_Loading opens, start async load
        SceneLoader.Instance.StartLoadingTarget();
    }
}
