using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Slider fullnessSlider;
    public Image fullnessSliderFill; // 슬라이더 Fill 이미지 연결
    public float maxFullness = 100f;
    private float currentFullness = 100f; // 100으로 시작

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentFullness = maxFullness; // 100으로 시작
        UpdateFullnessUI();
    }

    void Update()
    {
        if (currentFullness > 0)
        {
            currentFullness -= 1 * Time.deltaTime;
            UpdateFullnessUI();
        }
        else
        {
            currentFullness = 0;
            UpdateFullnessUI();
            SceneManager.LoadScene("GameOver"); // GameOver 씬으로 이동
        }
    }

    public void IncreaseFullness(float amount)
    {
        currentFullness += amount;
        if (currentFullness > maxFullness)
        {
            currentFullness = maxFullness;
        }
        Debug.Log("만복도 증가! 현재 만복도: " + currentFullness);
        UpdateFullnessUI();
    }

    void UpdateFullnessUI()
    {
        if (fullnessSlider != null)
        {
            fullnessSlider.maxValue = maxFullness;
            fullnessSlider.value = currentFullness;
        }
        // 색상 변화: 초록(100)→노랑(50)→빨강(0)
        if (fullnessSliderFill != null)
        {
            float t = currentFullness / maxFullness;
            // 0~0.5: 빨강→노랑, 0.5~1: 노랑→초록
            Color color;
            if (t < 0.5f)
            {
                // 빨강(1,0,0)→노랑(1,1,0)
                color = Color.Lerp(Color.red, Color.yellow, t * 2f);
            }
            else
            {
                // 노랑(1,1,0)→초록(0,1,0)
                color = Color.Lerp(Color.yellow, Color.green, (t - 0.5f) * 2f);
            }
            fullnessSliderFill.color = color;
        }
    }
}