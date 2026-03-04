using UnityEngine;

public class AutoDestroyAfterSeconds : MonoBehaviour
{
    public float lifeTime = 0.7f;
    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Kill), lifeTime);
    }
    void Kill()
    {
        Destroy(gameObject);
    }
}
