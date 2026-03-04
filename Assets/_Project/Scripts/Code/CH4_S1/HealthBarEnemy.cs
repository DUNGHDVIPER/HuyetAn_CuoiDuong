using UnityEngine;
using UnityEngine.UI;
public class HealthBarEnemy : MonoBehaviour
{
    public Slider slider; 
    public Vector3 offset;
    public void SetHealth(float health, float maxHealth)
    {
        slider.maxValue = maxHealth; slider.value = health;
    }
    //void LateUpdate()
    //{
    //    if (transform.parent != null)
    //    {
    //        transform.position = transform.parent.position + offset;
    //    }
    //}
}