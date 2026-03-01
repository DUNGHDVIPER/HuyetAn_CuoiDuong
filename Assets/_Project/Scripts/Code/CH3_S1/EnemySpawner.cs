using UnityEngine;
using System.Collections;

public class EnemyTunnelSpawner : MonoBehaviour
{
    [Header("Refs")]
    public GameObject enemyPrefab;
    public Transform player;

    [Header("Tunnel Points (pair)")]
    public Transform[] spawnInsidePoints; // điểm trong hầm
    public Transform[] exitMouthPoints;   // điểm cửa hầm (cùng index)

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public int maxAlive = 5;

    [Header("Emerge")]
    public float emergeTime = 0.6f;

    int alive = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (alive < maxAlive)
                SpawnOneFromRandomTunnel();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneFromRandomTunnel()
    {
        if (enemyPrefab == null) return;
        if (spawnInsidePoints == null || exitMouthPoints == null) return;
        if (spawnInsidePoints.Length == 0 || exitMouthPoints.Length == 0) return;

        int i = Random.Range(0, Mathf.Min(spawnInsidePoints.Length, exitMouthPoints.Length));

        Transform inside = spawnInsidePoints[i];
        Transform mouth = exitMouthPoints[i];

        // Spawn trong hầm
        GameObject e = Instantiate(enemyPrefab, inside.position, Quaternion.identity);
        alive++;

        // Gán player cho AI
        var ai = e.GetComponent<EnemyAI2D>();
        if (ai != null) ai.player = player;

        StartCoroutine(EmergeRoutine(e, mouth.position));
    }

    IEnumerator EmergeRoutine(GameObject enemy, Vector3 targetPos)
    {
        Transform t = enemy.transform;

        // tắt physics + collider + AI lúc "chui ra"
        var rb = enemy.GetComponent<Rigidbody2D>();
        var col = enemy.GetComponent<Collider2D>();
        var ai = enemy.GetComponent<EnemyAI2D>();

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

        // bật lại để bắt đầu đuổi đánh
        if (rb) rb.simulated = true;
        if (col) col.enabled = true;
        if (ai) ai.enabled = true;
    }
}