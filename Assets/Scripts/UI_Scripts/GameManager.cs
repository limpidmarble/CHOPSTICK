using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; 
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Slider fullnessSlider;
    public Image fullnessSliderFill;
    public FaceController faceController;
    public GameObject scorePopupPrefab;
    public AudioSource audioSource;
    public AudioClip eatSound;
    public float maxFullness = 100f;
    private float currentFullness = 100f;
    private float updateInterval = 0.01f;
    private float updateTimer = 0f;
    private bool isGameOver = false;

    private float elapsedTime = 0f;

    // 누적 점수 관련
    public TextMeshProUGUI scoreText; // UI에 할당
    private int totalScore = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentFullness = maxFullness;
        UpdateFullnessUI();
        faceController.UpdateFace(currentFullness, maxFullness);
        UpdateScoreUI();
    }

    void Update()
    {
    if (isGameOver) return;

    updateTimer += Time.deltaTime;
    elapsedTime += Time.deltaTime;

    if (updateTimer >= updateInterval)
    {
        updateTimer = 0f;

        float decreasePerSecond = Mathf.Min(elapsedTime * 0.25f + 2f, 12f);

        float decreaseAmount = decreasePerSecond * updateInterval;

        if (currentFullness > 0)
        {
            currentFullness -= decreaseAmount;
            if (currentFullness < 0) currentFullness = 0;
            UpdateFullnessUI();
            faceController.UpdateFace(currentFullness, maxFullness);
        }
        else
        {
            currentFullness = 0;
            UpdateFullnessUI();
            StartCoroutine(GameOverSequence());
        }
    }
}
    IEnumerator GameOverSequence()
    {
        isGameOver = true;
        faceController.SetGameOverFace();
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f;
        int i = 1;
        string prefKey;

        while (i <= 101)
        {
            if (i == 101)
            {
                i = 1;
                prefKey = "score" + $"{i}";
                PlayerPrefs.DeleteKey(prefKey);
                PlayerPrefs.SetInt(prefKey, totalScore);
                totalScore = 0;
                break;
            }
            prefKey = "score" + $"{i}";
            if (PlayerPrefs.HasKey(prefKey))
            {
                i++;
            }
            else
            {
                PlayerPrefs.SetInt(prefKey, totalScore);
                i++;
                totalScore = 0;
                break;
            }
        }

        SceneManager.LoadScene("GameOver");

    }


    public void IncreaseFullness(float amount, Vector3 foodPos, int score)
    {
        currentFullness += amount;
        if (currentFullness > maxFullness)
            currentFullness = maxFullness;
        UpdateFullnessUI();
        faceController.PlayEatAnimation();
        faceController.UpdateFace(currentFullness, maxFullness);
        ShowScorePopup(foodPos, score);
        if (audioSource && eatSound) audioSource.PlayOneShot(eatSound);

        // 점수 누적
        if (score > 0)
        {
            totalScore += score;
            UpdateScoreUI();
        }
    }

    public void ShowScorePopup(Vector3 worldPos, int score)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        GameObject popup = Instantiate(scorePopupPrefab, screenPos, Quaternion.identity, GameObject.Find("Canvas").transform);
        popup.GetComponent<ScorePopup>().Init(score);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }
    }

    void UpdateFullnessUI()
{
    if (fullnessSlider != null)
    {
        fullnessSlider.maxValue = maxFullness;
        fullnessSlider.value = currentFullness;
    }
    // 색상 변화: 빨강(0)→노랑(50)→녹차색(100)
    if (fullnessSliderFill != null)
{
    float t = currentFullness / maxFullness;
    Color greenTea = new Color(0.45f, 0.65f, 0.32f); // 진하지만 쨍하지 않은 녹차색
    Color color;
    if (t < 0.5f)
    {
        color = Color.Lerp(Color.red, Color.yellow, t * 2f);
    }
    else
    {
        color = Color.Lerp(Color.yellow, greenTea, (t - 0.5f) * 2f);
    }
    fullnessSliderFill.color = color;
}
}

    public float CurrentFullness => currentFullness;
    public int TotalScore => totalScore;
}

