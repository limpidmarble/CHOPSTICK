using UnityEngine;
using UnityEngine.UI;

public class FullnessBar : MonoBehaviour
{
    [SerializeField] private Slider fullnessSlider;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        if (fullnessSlider != null)
            fullnessSlider.onValueChanged.AddListener(UpdateBarColor);
        UpdateBarColor(fullnessSlider.value);
    }

    private void UpdateBarColor(float value)
    {
        // value: 0 (empty) ~ 1 (full)
        Color color;
        if (value < 0.5f)
        {
            // 빨강 → 노랑
            color = Color.Lerp(Color.red, Color.yellow, value * 2f);
        }
        else
        {
            // 노랑 → 초록
            color = Color.Lerp(Color.yellow, Color.green, (value - 0.5f) * 2f);
        }
        if (fillImage != null)
            fillImage.color = color;
    }
}