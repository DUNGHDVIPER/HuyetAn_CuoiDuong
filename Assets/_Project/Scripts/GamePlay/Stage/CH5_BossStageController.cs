using System.Collections;
using UnityEngine;

public class CH5_BossStageController : MonoBehaviour
{
    [Header("Pre-boss Waves (dùng pattern Map1)")]
    public Transform spawnLeft;
    public Transform spawnRight;
    public GameObject enemyPrefab; // dùng cho wave thường

    [Header("Boss Setup")]
    public GameObject bossPrefab;
    public Transform bossSpawn;

    [Header("Summon Gates")]
    public Transform leftGate;
    public Transform rightGate;
    public GameObject addPrefab;

    [Header("Fairness")]
    public float telegraphSeconds = 0.8f;
    public float minDistanceFromPlayer = 3.0f;
    public float randomOffsetRadius = 1.2f;

    [Header("UI (optional)")]
    public StageHUD hud;

    private StageRule _stageRule;
    private Transform _player;

    private int _aliveWaveEnemies;
    private int _addsAlive;
    private EnemyHealth _bossHp;
    private bool _altLeft = true; // luân phiên L/R cho P1

    private void Start()
    {
        _stageRule = FindFirstObjectByType<StageRule>();
        if (hud == null) hud = FindFirstObjectByType<StageHUD>();

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;

        if (addPrefab == null) addPrefab = enemyPrefab;

        StartCoroutine(RunBossStage());
    }

    private void Update()
    {
        // update boss HP UI
        if (_bossHp != null && !_bossHp.IsDead && hud != null)
            hud.SetBossHP(_bossHp.currentHP, _bossHp.maxHP);
    }

    private IEnumerator RunBossStage()
    {
        // ===== Pre-boss W1/W2/W3 như Map1 =====
        yield return RunCountWave(1, 3, count: 6, interval: 0.8f, waveMaxAlive: 4); // W1
        yield return new WaitForSeconds(3f);

        yield return RunCountWave(2, 3, count: 7, interval: 0.9f, waveMaxAlive: 5); // W2
        yield return new WaitForSeconds(3f);

        yield return RunCountWave(3, 3, count: 8, interval: 1.0f, waveMaxAlive: 6); // W3

        // Rest 4s + BossWarning 2s
        if (hud != null) hud.ShowWarning("REST", 0.8f);
        yield return new WaitForSeconds(4f);

        if (hud != null) hud.ShowWarning("BOSS INCOMING!", 2.0f);
        yield return new WaitForSeconds(2f);

        // ===== Spawn Boss =====
        SpawnBoss();

        if (_bossHp == null)
        {
            Debug.LogError("[CH5_B1] Boss missing EnemyHealth => không chạy phase/summon được.");
            yield break;
        }

        // ===== Boss loop (phase + summon + ulti) =====
        yield return StartCoroutine(BossPhasesLoop());
    }

    private IEnumerator RunCountWave(int waveIndex, int totalWaves, int count, float interval, int waveMaxAlive)
    {
        if (hud != null) { hud.SetWave(waveIndex, totalWaves); hud.ClearBossHP(); }

        int spawned = 0;
        while (spawned < count)
        {
            if (StageResultUI.IsShowingResult) yield break;

            if (_aliveWaveEnemies < waveMaxAlive)
            {
                SpawnWaveEnemy(enemyPrefab);
                spawned++;
            }

            yield return new WaitForSeconds(interval);
        }

        while (_aliveWaveEnemies > 0)
        {
            if (StageResultUI.IsShowingResult) yield break;
            yield return null;
        }
    }

    private void SpawnWaveEnemy(GameObject prefab)
    {
        Transform sp = (Random.value < 0.5f) ? spawnLeft : spawnRight;
        if (sp == null) sp = this.transform;

        var go = Instantiate(prefab, sp.position, Quaternion.identity);
        var hp = go.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            _aliveWaveEnemies++;
            hp.OnDeath += () => { _aliveWaveEnemies--; };
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null || bossSpawn == null)
        {
            Debug.LogError("[CH5_B1] bossPrefab/bossSpawn NULL");
            return;
        }

        var boss = Instantiate(bossPrefab, bossSpawn.position, Quaternion.identity);
        _bossHp = boss.GetComponent<EnemyHealth>();

        if (_bossHp != null)
        {
            _bossHp.OnDeath += OnBossDeath;
        }

        if (hud != null)
        {
            hud.SetWaveText("BOSS: Veyra");
            hud.SetBossHP(_bossHp != null ? _bossHp.currentHP : 0, _bossHp != null ? _bossHp.maxHP : 0);
        }
    }

    private IEnumerator BossPhasesLoop()
    {
        // timers
        float summonTimer = 0f;
        float ultiTimer = Random.Range(18f, 22f);
        bool altLeft = true;

        while (_bossHp != null && !_bossHp.IsDead)
        {
            if (StageResultUI.IsShowingResult) yield break;

            float hpPct = (_bossHp.maxHP > 0) ? (float)_bossHp.currentHP / _bossHp.maxHP : 0f;

            // phase config
            float summonInterval;
            int summonSides;      // 1 = luân phiên, 2 = cả 2 bên
            int maxAddsAlive;

            if (hpPct > 0.70f)
            {
                // P1
                summonInterval = 14f;
                summonSides = 1;
                maxAddsAlive = 6;
            }
            else if (hpPct > 0.35f)
            {
                // P2
                summonInterval = 10f;
                summonSides = 2;
                maxAddsAlive = 6;
            }
            else
            {
                // P3
                summonInterval = 8f;
                summonSides = 2;
                maxAddsAlive = 8;
            }

            // tick
            summonTimer -= Time.deltaTime;
            ultiTimer -= Time.deltaTime;

            // Ulti P3: 18–22s + downtime 1.8s
            if (hpPct <= 0.35f && ultiTimer <= 0f)
            {
                yield return DoUlti();
                ultiTimer = Random.Range(18f, 22f);

                // downtime 1.8s: ép summonTimer >= 1.8
                summonTimer = Mathf.Max(summonTimer, 1.8f);
            }

            if (summonTimer <= 0f)
            {
                // fairness cap
                if (_addsAlive < maxAddsAlive)
                {
                    // bỏ bool altLeft local luôn
                  
yield return SummonAdds(summonSides, maxAddsAlive);
                }
                else
                {
                    // cap reached => không spawn thêm (đúng spec)
                    if (hud != null) hud.ShowWarning("ADDS CAP - PRESSURE", 0.8f);
                }

                summonTimer = summonInterval;
            }

            yield return null;
        }
    }

    private IEnumerator DoUlti()
    {
        if (hud != null) hud.ShowWarning("VEYRA ULTI!", 1.0f);
        // Ở project hiện tại chưa có boss skill thật => tối thiểu: warning + debug
        Debug.Log("[CH5_B1] Veyra Ulti fired (placeholder)");

        // downtime “cảm giác”: 0.3s telegraph + 0.5s impact + 1.0s recover ~ tổng 1.8s handled ở loop
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator SummonAdds(int sides, int maxAddsAlive)
    {
        if (leftGate == null || rightGate == null)
        {
            Debug.LogWarning("[CH5_B1] leftGate/rightGate NULL");
            yield break;
        }

        if (hud != null) hud.ShowWarning("SUMMON!", 0.5f);

        // sides == 1: luân phiên L/R
        if (sides == 1)
        {
            Transform gate = _altLeft ? leftGate : rightGate;
            _altLeft = !_altLeft;

            if (_addsAlive < maxAddsAlive)
                yield return SpawnAddWithTelegraph(gate);
        }
        else
        {
            // sides == 2: 2 bên (nếu còn slot)
            if (_addsAlive <= maxAddsAlive - 2)
            {
                yield return SpawnAddWithTelegraph(leftGate);
                yield return SpawnAddWithTelegraph(rightGate);
            }
            else
            {
                Debug.Log("[CH5_B1] Not enough add slots for 2-side summon, skip.");
            }
        }
    }

    private IEnumerator SpawnAddWithTelegraph(Transform gate)
    {
        if (addPrefab == null || gate == null) yield break;

        // telegraph 0.8s (không có VFX prefab => chỉ delay; nếu bạn có marker prefab thì đặt ở đây)
        yield return new WaitForSeconds(telegraphSeconds);

        Vector3 pos = PickFairSpawnPosition(gate.position);
        var go = Instantiate(addPrefab, pos, Quaternion.identity);

        var hp = go.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            _addsAlive++;
            hp.OnDeath += () => { _addsAlive--; };
        }
    }

    private Vector3 PickFairSpawnPosition(Vector3 basePos)
    {
        if (_player == null) return basePos;

        for (int i = 0; i < 6; i++)
        {
            Vector2 offset = Random.insideUnitCircle * randomOffsetRadius;
            Vector3 p = basePos + new Vector3(offset.x, offset.y, 0f);

            if (Vector3.Distance(p, _player.position) >= minDistanceFromPlayer)
                return p;
        }

        // nếu fail nhiều lần => fallback basePos (nhưng vẫn có telegraph)
        return basePos;
    }

    private void OnBossDeath()
    {
        Debug.Log("[CH5_B1] Boss dead");

        // ưu tiên UnlockWinZone theo StageRule hiện có
        if (_stageRule != null)
        {
            if (_stageRule.winZone != null) _stageRule.UnlockWinZone();
            else _stageRule.TriggerWin("Boss dead (winZone missing)");
        }
    }
}