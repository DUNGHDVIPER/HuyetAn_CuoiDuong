using System.Collections;
using UnityEngine;

namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class PortalSpawnerEnemy : MonoBehaviour
    {
        [System.Serializable]
        public class PortalData
        {
            public GameObject portal;
            public GameObject enemyPrefab;

            [HideInInspector] public Transform spawnPoint;
            [HideInInspector] public SpriteRenderer spriteRenderer;
            [HideInInspector] public bool isSpawning;
        }

        public PortalData portalLeft;
        public PortalData portalRight;

        public float spawnDelay = 3f;
        public int maxEnemies = 10;

        private int currentEnemies = 0;

        void Start()
        {
            InitPortal(portalLeft);
            InitPortal(portalRight);

            StartCoroutine(SpawnLoop());
        }

        void InitPortal(PortalData p)
        {
            if (p.portal == null) return;

            p.spriteRenderer = p.portal.GetComponent<SpriteRenderer>();
            p.spawnPoint = p.portal.transform.Find("SpawnPoint");

            if (p.spawnPoint == null)
            {
                Debug.LogError("Không tìm thấy SpawnPoint trong " + p.portal.name);
            }

            p.portal.SetActive(false);

            if (p.spriteRenderer != null)
            {
                Color c = p.spriteRenderer.color;
                c.a = 0;
                p.spriteRenderer.color = c;
            }
        }

        IEnumerator SpawnLoop()
        {
            while (true)
            {
                if (currentEnemies < maxEnemies)
                {
                    yield return StartCoroutine(SpawnAtPortal(portalLeft));
                    yield return new WaitForSeconds(0.5f);
                    yield return StartCoroutine(SpawnAtPortal(portalRight));
                }

                yield return new WaitForSeconds(spawnDelay);
            }
        }

        IEnumerator SpawnAtPortal(PortalData p)
        {
            if (p.isSpawning || p.portal == null || p.spawnPoint == null)
                yield break;

            if (currentEnemies >= maxEnemies)
                yield break;

            if (p.enemyPrefab == null)
            {
                Debug.LogError("Enemy Prefab chưa gán cho " + p.portal.name);
                yield break;
            }

            p.isSpawning = true;

            // Mở portal
            p.portal.SetActive(true);
            yield return StartCoroutine(Fade(p.spriteRenderer, 0f, 1f, 0.25f));
            yield return new WaitForSeconds(0.15f);

            // Spawn thấp hơn để tạo hiệu ứng chui lên
            Vector3 startPos = p.spawnPoint.position + Vector3.down * 2f;

            GameObject enemy = Instantiate(
                p.enemyPrefab,
                startPos,
                Quaternion.identity
            );

            currentEnemies++;

            // Tắt collider khi đang trồi lên
            Collider2D col = enemy.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // Trồi lên
            yield return StartCoroutine(RiseUp(enemy.transform, p.spawnPoint.position));

            // Bật lại collider
            if (col != null) col.enabled = true;

            // Gắn event chết
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.OnDeath += HandleEnemyDeath;
            }

            yield return new WaitForSeconds(0.2f);

            // Đóng portal
            yield return StartCoroutine(Fade(p.spriteRenderer, 1f, 0f, 0.25f));
            p.portal.SetActive(false);

            p.isSpawning = false;
        }

        IEnumerator RiseUp(Transform enemy, Vector3 targetPos)
        {
            float duration = 0.4f;
            float t = 0f;
            Vector3 startPos = enemy.position;

            while (t < duration)
            {
                t += Time.deltaTime;
                float smooth = Mathf.SmoothStep(0, 1, t / duration);
                enemy.position = Vector3.Lerp(startPos, targetPos, smooth);
                yield return null;
            }

            enemy.position = targetPos;
        }

        void HandleEnemyDeath()
        {
            currentEnemies--;
        }

        IEnumerator Fade(SpriteRenderer sr, float from, float to, float time)
        {
            if (sr == null) yield break;

            float t = 0f;
            Color c = sr.color;

            while (t < time)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(from, to, t / time);
                sr.color = c;
                yield return null;
            }

            c.a = to;
            sr.color = c;
        }
    }
}