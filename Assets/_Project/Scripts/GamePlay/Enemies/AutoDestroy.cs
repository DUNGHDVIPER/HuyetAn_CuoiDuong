using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float life = 0.35f;
    void Start() => Destroy(gameObject, life);
}