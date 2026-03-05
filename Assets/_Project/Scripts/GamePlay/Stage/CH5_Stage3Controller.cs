using System.Collections;
using UnityEngine;

public class CH5_Stage3Controller : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform spawnLeft;
    public Transform spawnRight;

    [Header("Enemy Prefabs")]
    public GameObject meleePrefab;
    public GameObject bomberPrefab;
    public GameObject brutePrefab;

    [Header("UI (optional)")]
    public StageHUD hud;

    [Header("Telegraph (Portal Ping)")]
    public bool enablePortalTelegraph = true;
    public float telegraphDelayMin = 0.22f;
    public float telegraphDelayMax = 0.35f;

    private StageRule _stageRule;
    private int _alive;
    private float _elapsed;

    private void Start()
    {
        _stageRule = FindFirstObjectByType<StageRule>();
        if (hud == null) hud = FindFirstObjectByType<StageHUD>();

        if (meleePrefab == null)
        {
            Debug.LogError("[CH5_S3] meleePrefab NULL (assign in inspector)");
            return;
        }
        if (bomberPrefab == null) bomberPrefab = meleePrefab;
        if (brutePrefab == null) brutePrefab = meleePrefab;

        StartCoroutine(RunMap3());
    }

    private void Update()
    {
        if (StageResultUI.IsShowingResult) return;
        _elapsed += Time.deltaTime;
        if (hud != null) hud.SetTimerSeconds(_elapsed);
    }

    private IEnumerator RunMap3()
    {
        // ✅ Wave1: Giới thiệu bomber (melee + bomber)
        yield return RunMixedCountWave(1, 4, interval: 0.9f, waveMaxAlive: 4,
            meleeCount: 3, bomberCount: 2, bruteCount: 0);

        // ✅ Wave2: Thêm brute (bomber + brute)
        yield return RunMixedCountWave(2, 4, interval: 1.0f, waveMaxAlive: 5,
            meleeCount: 2, bomberCount: 3, bruteCount: 1);

        // ✅ Wave3: AMBUSH bomber vs brute (spawn 2 bên)
        yield return RunAmbushPairsWave(3, 4, pairCount: 4, interval: 1.0f, waveMaxAlive: 6);

        // ✅ Wave4: Boss-like: brute nhiều hơn + bomber spam
        yield return RunMixedCountWave(4, 4, interval: 1.1f, waveMaxAlive: 6,
            meleeCount: 0, bomberCount: 4, bruteCount: 2);

        if (_stageRule != null) _stageRule.TriggerWin("CH5_S3 - All waves cleared");
    }

    // ---------------------------
    // Waves
    // ---------------------------

    private IEnumerator RunAmbushPairsWave(int waveIndex, int totalWaves, int pairCount, float interval, int waveMaxAlive)
    {
        if (hud != null) { hud.SetWave(waveIndex, totalWaves); hud.ShowWarning("AMBUSH!", 1.2f); }

        int spawnedPairs = 0;
        while (spawnedPairs < pairCount)
        {
            if (StageResultUI.IsShowingResult) yield break;

            // cần ít nhất 2 slot trống để spawn đồng thời
            if (_alive <= waveMaxAlive - 2)
            {
                // Ping cả 2 cổng rồi spawn cùng lúc
                yield return SpawnPairTelegraph(bomberPrefab, spawnLeft, brutePrefab, spawnRight);
                spawnedPairs++;
            }

            yield return new WaitForSeconds(interval);
        }

        while (_alive > 0) { if (StageResultUI.IsShowingResult) yield break; yield return null; }
    }

    private IEnumerator RunMixedCountWave(int waveIndex, int totalWaves, float interval, int waveMaxAlive,
        int meleeCount, int bomberCount, int bruteCount)
    {
        if (hud != null) hud.SetWave(waveIndex, totalWaves);

        int total = meleeCount + bomberCount + bruteCount;
        int spawned = 0;

        while (spawned < total)
        {
            if (StageResultUI.IsShowingResult) yield break;

            if (_alive < waveMaxAlive)
            {
                GameObject p = PickNextPrefab(ref meleeCount, ref bomberCount, ref bruteCount);
                yield return SpawnRandomSideTelegraph(p);
                spawned++;
            }

            yield return new WaitForSeconds(interval);
        }

        while (_alive > 0) { if (StageResultUI.IsShowingResult) yield break; yield return null; }
    }

    private GameObject PickNextPrefab(ref int melee, ref int bomber, ref int brute)
    {
        if (brute > 0) { brute--; return brutePrefab; }
        if (bomber > 0) { bomber--; return bomberPrefab; }
        melee--; return meleePrefab;
    }

    // ---------------------------
    // Spawn + Telegraph
    // ---------------------------

    private IEnumerator SpawnRandomSideTelegraph(GameObject prefab)
    {
        Transform sp = (Random.value < 0.5f) ? spawnLeft : spawnRight;
        yield return SpawnAtTelegraph(prefab, sp);
    }

    private IEnumerator SpawnPairTelegraph(GameObject leftPrefab, Transform leftSp, GameObject rightPrefab, Transform rightSp)
    {
        if (leftSp == null) leftSp = this.transform;
        if (rightSp == null) rightSp = this.transform;

        if (enablePortalTelegraph)
        {
            leftSp.GetComponentInChildren<SpawnPortalFX>(true)?.Ping();
            rightSp.GetComponentInChildren<SpawnPortalFX>(true)?.Ping();
            float d = Random.Range(telegraphDelayMin, telegraphDelayMax);
            yield return new WaitForSeconds(d);
        }

        SpawnImmediate(leftPrefab, leftSp);
        SpawnImmediate(rightPrefab, rightSp);
    }

    private IEnumerator SpawnAtTelegraph(GameObject prefab, Transform sp)
    {
        if (prefab == null)
        {
            Debug.LogError("[CH5_S3] Spawn prefab NULL (check inspector refs)");
            yield break;
        }
        if (sp == null) sp = this.transform;

        if (enablePortalTelegraph)
        {
            sp.GetComponentInChildren<SpawnPortalFX>(true)?.Ping();
            float d = Random.Range(telegraphDelayMin, telegraphDelayMax);
            yield return new WaitForSeconds(d);
        }

        SpawnImmediate(prefab, sp);
    }

    private void SpawnImmediate(GameObject prefab, Transform sp)
    {
        var go = Instantiate(prefab, sp.position, Quaternion.identity);

        var hp = go.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            _alive++;
            hp.OnDeath += () => { _alive = Mathf.Max(0, _alive - 1); };
        }
        else
        {
            Debug.LogWarning("[CH5_S3] Enemy missing EnemyHealth");
        }
    }
}