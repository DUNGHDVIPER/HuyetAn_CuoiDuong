using System.Collections;
using UnityEngine;

public class CH5_Stage2Controller : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform spawnLeft;
    public Transform spawnRight;

    [Header("Enemy Prefabs")]
    public GameObject meleePrefab;
    public GameObject rangedPrefab;
    public GameObject bomberPrefab;
    public GameObject brutePrefab;

    [Header("UI (optional)")]
    public StageHUD hud;

    [Header("Telegraph (Portal Ping)")]
    public bool enablePortalTelegraph = true;
    public float telegraphDelayMin = 0.22f;
    public float telegraphDelayMax = 0.35f;

    [Header("Map2 Ranger Bias (Hold wave)")]
    [Range(0f, 1f)] public float holdRangedChance = 0.65f;  // spam ranged
    [Range(0f, 1f)] public float holdBomberChance = 0.15f;  // thêm chút bomber
    // meleeChance = 1 - ranged - bomber (brute ít -> wave4)

    private StageRule _stageRule;
    private int _alive;
    private float _elapsed;

    private void Start()
    {
        _stageRule = FindFirstObjectByType<StageRule>();
        if (hud == null) hud = FindFirstObjectByType<StageHUD>();

        // fallback nếu thiếu prefab
        if (meleePrefab == null)
        {
            Debug.LogError("[CH5_S2] meleePrefab NULL (assign in inspector)");
            return;
        }
        if (rangedPrefab == null) rangedPrefab = meleePrefab;
        if (bomberPrefab == null) bomberPrefab = meleePrefab;
        if (brutePrefab == null) brutePrefab = meleePrefab;

        StartCoroutine(RunMap2());
    }

    private void Update()
    {
        if (StageResultUI.IsShowingResult) return;
        _elapsed += Time.deltaTime;
        if (hud != null) hud.SetTimerSeconds(_elapsed);
    }

    private IEnumerator RunMap2()
    {
        // ✅ Wave1: 3 melee + 3 ranged (đưa ranged vào sớm)
        yield return RunMixedCountWave(1, 4, interval: 0.85f, waveMaxAlive: 4,
            meleeCount: 3, rangedCount: 3, bomberCount: 0, bruteCount: 0);

        // ✅ Wave2: HOLD 20s (spam ranged chủ yếu)
        yield return RunHoldWaveWeighted(2, 4, holdSeconds: 20f, interval: 2.6f, waveMaxAlive: 5);

        // ✅ Wave3: ranged nhiều + thêm bomber
        yield return RunMixedCountWave(3, 4, interval: 0.95f, waveMaxAlive: 6,
            meleeCount: 2, rangedCount: 7, bomberCount: 2, bruteCount: 0);

        // ✅ Wave4: ranged + bomber + 1 brute (mini “elite”)
        yield return RunMixedCountWave(4, 4, interval: 1.05f, waveMaxAlive: 6,
            meleeCount: 0, rangedCount: 5, bomberCount: 2, bruteCount: 1);

        if (_stageRule != null) _stageRule.TriggerWin("CH5_S2 - All waves cleared");
    }

    // ---------------------------
    // Waves
    // ---------------------------

    private IEnumerator RunMixedCountWave(int waveIndex, int totalWaves, float interval, int waveMaxAlive,
        int meleeCount, int rangedCount, int bomberCount, int bruteCount)
    {
        if (hud != null) hud.SetWave(waveIndex, totalWaves);

        int total = meleeCount + rangedCount + bomberCount + bruteCount;
        int spawned = 0;

        while (spawned < total)
        {
            if (StageResultUI.IsShowingResult) yield break;

            if (_alive < waveMaxAlive)
            {
                GameObject p = PickNextPrefab(ref meleeCount, ref rangedCount, ref bomberCount, ref bruteCount);
                yield return SpawnTelegraph(p, randomSide: true);
                spawned++;
            }

            yield return new WaitForSeconds(interval);
        }

        while (_alive > 0)
        {
            if (StageResultUI.IsShowingResult) yield break;
            yield return null;
        }
    }

    private IEnumerator RunHoldWaveWeighted(int waveIndex, int totalWaves, float holdSeconds, float interval, int waveMaxAlive)
    {
        if (hud != null) { hud.SetWave(waveIndex, totalWaves); hud.ShowWarning("HOLD", 1.0f); }

        float endTime = Time.time + holdSeconds;

        while (Time.time < endTime)
        {
            if (StageResultUI.IsShowingResult) yield break;

            if (_alive < waveMaxAlive)
            {
                GameObject p = PickHoldPrefab();
                yield return SpawnTelegraph(p, randomSide: true);
            }

            yield return new WaitForSeconds(interval);
        }

        while (_alive > 0)
        {
            if (StageResultUI.IsShowingResult) yield break;
            yield return null;
        }
    }

    private GameObject PickNextPrefab(ref int melee, ref int ranged, ref int bomber, ref int brute)
    {
        if (brute > 0) { brute--; return brutePrefab; }
        if (ranged > 0) { ranged--; return rangedPrefab; }
        if (bomber > 0) { bomber--; return bomberPrefab; }
        melee--; return meleePrefab;
    }

    private GameObject PickHoldPrefab()
    {
        // ranged spam mạnh
        float r = Random.value;

        float rangedT = holdRangedChance;
        float bomberT = holdRangedChance + holdBomberChance;

        if (r < rangedT) return rangedPrefab;
        if (r < bomberT) return bomberPrefab;
        return meleePrefab;
    }

    // ---------------------------
    // Spawn + Telegraph
    // ---------------------------

    private IEnumerator SpawnTelegraph(GameObject prefab, bool randomSide)
    {
        if (prefab == null)
        {
            Debug.LogError("[CH5_S2] Spawn prefab NULL (check inspector refs)");
            yield break;
        }

        Transform sp = spawnLeft;
        if (randomSide) sp = (Random.value < 0.5f) ? spawnLeft : spawnRight;
        if (sp == null) sp = this.transform;

        // Ping portal
        if (enablePortalTelegraph)
        {
            var fx = sp.GetComponentInChildren<SpawnPortalFX>(true);
            fx?.Ping();
            float d = Random.Range(telegraphDelayMin, telegraphDelayMax);
            yield return new WaitForSeconds(d);
        }

        var go = Instantiate(prefab, sp.position, Quaternion.identity);

        // debug giúp bạn thấy đang spawn cái gì
        go.name = prefab.name;
        Debug.Log($"[CH5_S2] Spawned: {prefab.name}");

        _alive++;
        StartCoroutine(TrackAlive(go));
    }

    private IEnumerator TrackAlive(GameObject go)
    {
        if (go == null) { _alive = Mathf.Max(0, _alive - 1); yield break; }

        var hp = go.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            while (go != null && !hp.IsDead) yield return null;
        }
        else
        {
            while (go != null) yield return null;
        }

        _alive = Mathf.Max(0, _alive - 1);
    }
}