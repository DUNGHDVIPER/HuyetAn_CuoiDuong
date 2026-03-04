using UnityEngine;

public class DamageHitbox2D : MonoBehaviour
{
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Đánh vào enemy có Health
        Health hp = other.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}
