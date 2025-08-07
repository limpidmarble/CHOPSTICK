using UnityEngine;

public class TopCollider : MonoBehaviour
{
    public GameObject successEffect;
    public AudioClip successSound;
    private AudioSource audioSource;
    public FoodSpawner foodSpawner;

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            Food food = other.GetComponent<Food>();
        
            GameManager.instance.IncreaseFullness(food.data.fullnessValue, other.transform.position, food.data.score);
           

            if (successEffect != null)
                Instantiate(successEffect, other.transform.position, Quaternion.identity);

            if (successSound != null)
                audioSource.PlayOneShot(successSound);

            Destroy(other.gameObject);
            foodSpawner.foodInScene--;
        }
    }
}