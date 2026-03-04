using UnityEngine;

public class LevelManager_CH3 : MonoBehaviour
{
    public GameObject portal;

    private int enemyCount;

    void Start()
    {
        // Đếm số quái ban đầu trong scene
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        // Ẩn portal lúc đầu
        if (portal != null)
            portal.SetActive(false);
    }

    public void EnemyDied()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            if (portal != null)
                portal.SetActive(true);
        }
    }
}