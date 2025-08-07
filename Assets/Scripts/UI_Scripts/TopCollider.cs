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
            GameManager.instance.IncreaseFullness(10, other.transform.position);

            if (successEffect != null)
                Instantiate(successEffect, other.transform.position, Quaternion.identity);

            if (successSound != null)
                audioSource.PlayOneShot(successSound);

            Destroy(other.gameObject);
            foodSpawner.foodInScene--;
        }
    }
}