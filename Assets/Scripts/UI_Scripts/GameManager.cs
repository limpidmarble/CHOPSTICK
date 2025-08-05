using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; 
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
    }

    void Update()
    {
        if (isGameOver) return;

        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            if (currentFullness > 0)
            {
                currentFullness -= 3 * updateInterval;
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
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOver");
    }

    public void IncreaseFullness(float amount, Vector3 foodPos)
    {
        currentFullness += amount;
        if (currentFullness > maxFullness)
            currentFullness = maxFullness;
        UpdateFullnessUI();
        faceController.PlayEatAnimation();
        faceController.UpdateFace(currentFullness, maxFullness);
        ShowScorePopup(foodPos, (int)amount);
        if (audioSource && eatSound) audioSource.PlayOneShot(eatSound);
    }

    public void ShowScorePopup(Vector3 worldPos, int score)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        GameObject popup = Instantiate(scorePopupPrefab, screenPos, Quaternion.identity, GameObject.Find("Canvas").transform);
        popup.GetComponent<ScorePopup>().Init(score);
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
            Color color;
            if (t < 0.5f)
            {
                color = Color.Lerp(Color.red, Color.yellow, t * 2f);
            }
            else
            {
                color = Color.Lerp(Color.yellow, Color.green, (t - 0.5f) * 2f);
            }
            fullnessSliderFill.color = color;
        }
    }

    public float CurrentFullness => currentFullness;
}