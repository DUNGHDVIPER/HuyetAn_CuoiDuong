using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;

    public Color fullHPColor = Color.green;
    public Color midHPColor = Color.yellow;
    public Color lowHPColor = Color.red;

    public void SetMaxHP(int maxHP)
    {
        slider.maxValue = maxHP;
        slider.value = maxHP;
        UpdateColor();
    }

    public void SetHP(int hp)
    {
        slider.value = hp;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (fillImage == null || slider.maxValue <= 0) return;

        float percent = slider.value / slider.maxValue;

        if (percent > 0.6f)
            fillImage.color = fullHPColor;
        else if (percent > 0.3f)
            fillImage.color = midHPColor;
        else
            fillImage.color = lowHPColor;
    }
}