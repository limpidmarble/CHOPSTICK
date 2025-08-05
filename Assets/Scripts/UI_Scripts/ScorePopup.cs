using UnityEngine;
using TMPro;
using System.Collections;





public class ScorePopup : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void Init(int score)
    {
        scoreText.text = $"+{score}";
        StartCoroutine(PopupAnim());
    }

    private IEnumerator PopupAnim()
    {
        float duration = 3f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 1.2f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            scoreText.alpha = Mathf.Lerp(1f, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
