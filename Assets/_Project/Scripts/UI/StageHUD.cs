/*using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageHUD : MonoBehaviour
{
    public TextMeshProUGUI title;

    private void Start()
    {
        // ví dụ scene name: "CH5_S1"
        string scene = SceneManager.GetActiveScene().name;

        // cách 1: hiện đúng scene name
        if (title != null) title.text = scene;
    }
}
*/
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageHUD : MonoBehaviour
{
    public TextMeshProUGUI title;

    private void Start()
    {
        string scene = SceneManager.GetActiveScene().name; // "CH5_S1"
        if (title == null) return;

        // default
        title.text = scene;

        // parse đơn giản
        // CH5_S1 => ch=5 stage=1
        if (scene.StartsWith("CH") && scene.Contains("_S"))
        {
            string chStr = scene.Substring(2, scene.IndexOf("_") - 2); // "5"
            string sStr = scene.Substring(scene.IndexOf("_S") + 2);    // "1"
            title.text = $"Chapter {chStr} - Stage {sStr}";
        }
        else if (scene.StartsWith("CH") && scene.Contains("_BOSS"))
        {
            string chStr = scene.Substring(2, scene.IndexOf("_") - 2);
            title.text = $"Chapter {chStr} - BOSS";
        }
    }
}
