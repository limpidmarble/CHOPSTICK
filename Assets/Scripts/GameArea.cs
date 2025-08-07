using Unity.Mathematics;
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
        if (collision.CompareTag("IButton"))
        {
            collision.transform.position = new Vector3(0f, 4f, 0f);
            collision.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 5f));
            collision.attachedRigidbody.angularVelocity = 0f;
            collision.attachedRigidbody.linearVelocity = Vector3.zero;
            Debug.Log("나갔네");
        }
    }
}
