using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal_CH3 : MonoBehaviour
{
    public string nextSceneName = "CH3_S2";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}