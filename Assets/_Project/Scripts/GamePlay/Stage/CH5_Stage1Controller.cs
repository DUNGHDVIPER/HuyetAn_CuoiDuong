using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CH5_Stage1Controller : MonoBehaviour
{
    [Header("Spawn Points (đang có trong scene)")]
    public Transform spawnLeft;
    public Transform spawnRight;

    [Header("Enemy Prefab (đang dùng được)")]
    public GameObject enemyPrefab;

    [Header("Legacy fields (GIỮ để không mất wiring cũ)")]
    public int totalEnemies = 15;
    public int maxAlive = 3;
    public float spawnDelay = 2f;

    [Header("Wave Mode")]
    public bool useWaveTable = true;

    [Header("UI (optional)")]
    public StageHUD hud;

    private StageRule _stageRule;
    private int _alive;
    private float _elapsed;

    private void Start()
    {
        _stageRule = FindFirstObjectByType<StageRule>();
        if (hud == null) hud = FindFirstObjectByType<StageHUD>();

        if (useWaveTable) StartCoroutine(RunMap1Waves());
        else StartCoroutine(LegacySpawnLoop());
    }

    private void Update()
    {
        if (StageResultUI.IsShowingResult) return;
        _elapsed += Time.deltaTime;
        if (hud != null) hud.SetTimerSeconds(_elapsed);
    }

    // ===== Map1 waves (theo bảng bạn đưa) =====
    private IEnumerator RunMap1Waves()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[CH5_S1] enemyPrefab NULL");
            yield break;
        }

        // Wave1: 6, interval 0.8, maxAlive=4
        yield return RunCountWave(waveIndex: 1, totalWaves: 3, count: 6, interval: 0.8f, waveMaxAlive: 4);

        // Rest 3s
        yield return Rest(3f);

        // Wave2: 4 melee + 3 bomber => total 7, interval 0.9, maxAlive=5 (chưa có prefab phân loại => spawn enemyPrefab)
        yield return RunCountWave(waveIndex: 2, totalWaves: 3, count: 7, interval: 0.9f, waveMaxAlive: 5);

        // Rest 3s
        yield return Rest(3f);

        // Wave3: 4 melee + 3 ranged + 1 brute => total 8, interval 1.0, maxAlive=6
        yield return RunCountWave(waveIndex: 3, totalWaves: 3, count: 8, interval: 1.0f, waveMaxAlive: 6);

        // End => WIN
        if (_stageRule != null) _stageRule.TriggerWin("CH5_S1 - All waves cleared");
        else Debug.LogWarning("[CH5_S1] StageRule not found");
    }

    private IEnumerator Rest(float seconds)
    {
        if (hud != null) hud.ShowWarning("REST", 0.6f);
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator RunCountWave(int waveIndex, int totalWaves, int count, float interval, int waveMaxAlive)
    {
        if (hud != null) hud.SetWave(waveIndex, totalWaves);

        int spawned = 0;
        while (spawned < count)
        {
            if (StageResultUI.IsShowingResult) yield break;

            if (_alive < waveMaxAlive)
            {
                SpawnEnemy(enemyPrefab);
                spawned++;
            }

            yield return new WaitForSeconds(interval);
        }

        // chờ clear wave
        while (_alive > 0)
        {
            if (StageResultUI.IsShowingResult) yield break;
            yield return null;
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Transform sp = (Random.value < 0.5f) ? spawnLeft : spawnRight;
        if (sp == null) sp = this.transform;

        GameObject enemy = Instantiate(prefab, sp.position, Quaternion.identity);

        var hp = enemy.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            _alive++;
            hp.OnDeath += () => { _alive--; };
        }
        else
        {
            Debug.LogWarning("[CH5_S1] Spawned enemy missing EnemyHealth");
        }
    }

    // ===== Legacy (giữ nguyên behavior cũ nếu bạn muốn bật lại) =====
    private IEnumerator LegacySpawnLoop()
    {
        int spawned = 0;
        _alive = 0;

        while (spawned < totalEnemies)
        {
            if (_alive < maxAlive)
            {
                SpawnEnemy(enemyPrefab);
                spawned++;
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        while (_alive > 0) yield return null;

        StageRule stage = FindFirstObjectByType<StageRule>();
        if (stage != null) stage.TriggerWin("Legacy cleared");
    }
}