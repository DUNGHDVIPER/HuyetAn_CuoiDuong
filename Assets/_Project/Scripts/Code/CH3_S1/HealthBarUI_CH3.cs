using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Image fillImage;
    private int maxHP;

    void Awake()
    {
        fillImage = GetComponent<Image>(); // tự lấy Image
    }

    public void SetMaxHP(int max)
    {
        maxHP = max;
    }

    public void SetHP(int currentHP)
    {
        if (fillImage == null || maxHP <= 0) return;

        fillImage.fillAmount = (float)currentHP / maxHP;
    }
}