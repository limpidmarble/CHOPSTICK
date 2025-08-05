using UnityEngine;

public class Food : MonoBehaviour
{
    public int score = 3; // Inspector에서 음식별 점수 조절 가능

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            GameManager.instance.IncreaseFullness(score, transform.position);
            Destroy(gameObject);
        }
    }
}