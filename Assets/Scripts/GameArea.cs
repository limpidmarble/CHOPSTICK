using Unity.VisualScripting;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    public FoodSpawner food_spawner;


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            Destroy(collision.gameObject);
        }
    }
}
