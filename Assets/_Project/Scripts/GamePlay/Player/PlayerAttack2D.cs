using System.Collections;
using UnityEngine;

public class PlayerAttack2D : MonoBehaviour
{
    [Header("Hitbox")]
    public GameObject hitboxPrefab;       // prefab hitbox
    public Transform hitboxSpawnPoint;    // điểm spawn (trước mặt)
    public float hitboxLifeTime = 0.15f;

    [Header("Damage")]
    public int lightDamage = 2; // J
    public int heavyDamage = 4; // K

    private void Update()
    {
        // Keybind cố định
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnHitbox(lightDamage);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnHitbox(heavyDamage);
        }
    }

    private void SpawnHitbox(int dmg)
    {
        if (hitboxPrefab == null || hitboxSpawnPoint == null) return;

        GameObject hb = Instantiate(hitboxPrefab, hitboxSpawnPoint.position, Quaternion.identity);
        DamageHitbox2D dmgComp = hb.GetComponent<DamageHitbox2D>();
        if (dmgComp != null) dmgComp.damage = dmg;

        StartCoroutine(DestroyAfter(hb, hitboxLifeTime));
    }

    private IEnumerator DestroyAfter(GameObject obj, float t)
    {
        yield return new WaitForSeconds(t);
        if (obj != null) Destroy(obj);
    }
}
