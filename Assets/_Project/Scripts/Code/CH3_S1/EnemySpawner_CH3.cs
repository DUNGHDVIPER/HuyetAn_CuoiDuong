using UnityEngine;
using System.Collections;

public class EnemyTunnelSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Refs")]
    public Transform player;

    [Header("Tunnel Points (pair)")]
    public Transform[] spawnInsidePoints;
    public Transform[] exitMouthPoints;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public int maxAlive = 5;
    public int totalSpawnLimit = 10;   // 🔥 TỐI ĐA 10 CON

    [Header("Emerge")]
    public float emergeTime = 0.6f;

    private int alive = 0;
    private int totalSpawned = 0;
    private bool stopSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (!stopSpawning)
        {
            if (alive < maxAlive && totalSpawned < totalSpawnLimit)
            {
                SpawnOneFromRandomTunnel();
            }

            if (totalSpawned >= totalSpawnLimit)
            {
                stopSpawning = true;
                Debug.Log("Spawn limit reached.");
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneFromRandomTunnel()
    {
        if (enemyPrefabs.Length == 0) return;
        if (spawnInsidePoints.Length == 0 || exitMouthPoints.Length == 0) return;

        int tunnelIndex = Random.Range(0,
            Mathf.Min(spawnInsidePoints.Length, exitMouthPoints.Length));

        Transform inside = spawnInsidePoints[tunnelIndex];
        Transform mouth = exitMouthPoints[tunnelIndex];

        // Không spawn nếu có quái đứng ở miệng hầm
        Collider2D hit = Physics2D.OverlapCircle(
            mouth.position,
            1.2f,
            LayerMask.GetMask("Enemy")
        );

        if (hit != null)
            return;

        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[enemyIndex];

        GameObject e = Instantiate(prefabToSpawn, inside.position, Quaternion.identity);

        alive++;
        totalSpawned++;

        // Gán player cho AI
        var ai = e.GetComponent<EnemyAI2D_CH3>();
        if (ai != null)
            ai.player = player;

        // Theo dõi chết
        StartCoroutine(WatchEnemyDeath(e));

        // Hiệu ứng chui lên
        StartCoroutine(EmergeRoutine(e, mouth.position));
    }

    IEnumerator WatchEnemyDeath(GameObject enemy)
    {
        while (enemy != null)
        {
            var health = enemy.GetComponent<EnemyHealth1>();
            if (health != null && health.IsDead)
                break;

            yield return null;
        }

        alive = Mathf.Max(0, alive - 1);
    }

    IEnumerator EmergeRoutine(GameObject enemy, Vector3 targetPos)
    {
        Transform t = enemy.transform;

        var rb = enemy.GetComponent<Rigidbody2D>();
        var col = enemy.GetComponent<Collider2D>();
        var ai = enemy.GetComponent<EnemyAI2D_CH3>();

        if (rb) rb.simulated = false;
        if (col) col.enabled = false;
        if (ai) ai.enabled = false;

        Vector3 start = t.position;
        float time = 0f;

        while (time < emergeTime)
        {
            time += Time.deltaTime;
            float k = Mathf.Clamp01(time / emergeTime);
            t.position = Vector3.Lerp(start, targetPos, k);
            yield return null;
        }

        t.position = targetPos;

        if (rb) rb.simulated = true;
        if (col) col.enabled = true;
        if (ai) ai.enabled = true;
    }
}