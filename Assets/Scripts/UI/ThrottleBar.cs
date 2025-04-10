using SpaceGame.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceGame.UI
{
    public class ThrottleBar : MonoBehaviour
    {
        public Slider slider;
        public TMP_Text text;
        public Gradient gradient;
        public Image fill;


        //private void Start()
        //{
        //    SetMaxThrottle(1f);
        //}

        //private void Update()
        //{
        //    SetThrottle(plane.Throttle);
        //}

        public void SetMaxThrottle(float value)
        {
            slider.maxValue = value;
            slider.value = value;

            text.text = FormattedValue(value, slider.maxValue);

            fill.color = gradient.Evaluate(1f);
        }

        public void SetThrottle(float value)
        {
            slider.value = value;
            fill.color = gradient.Evaluate(slider.normalizedValue);
            text.text = FormattedValue(value, slider.maxValue);
        }

        private string FormattedValue(float value, float maxValue)
        {
            var health = slider.normalizedValue * 100;
            if (health % 1 == 0)
            {
                return Mathf.RoundToInt(health).ToString();
            }

            return health.ToString("0.##");
        }


    }
}
