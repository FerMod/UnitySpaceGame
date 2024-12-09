using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text text;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(float value)
    {
        slider.maxValue = value;
        slider.value = value;

        text.text = FormattedHealth(value, slider.maxValue);

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float value)
    {
        slider.value = value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        text.text = FormattedHealth(value, slider.maxValue);
    }

    private string FormattedHealth(float value, float maxValue)
    {
        var health = slider.normalizedValue * 100;
        if (health % 1 == 0)
        {
            return Mathf.RoundToInt(health).ToString();
        }

        return health.ToString("0.##");
    }
}
