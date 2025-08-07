using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Food : MonoBehaviour
{
    public FoodData data;

    private void Start()
    {
        // Sprite 적용
        GetComponent<SpriteRenderer>().sprite = data.sprite;
        // 질량 적용
        GetComponent<Rigidbody2D>().mass = Mathf.Ceil(data.mass * 0.5f);
        // 마찰 적용
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            PhysicsMaterial2D mat = new PhysicsMaterial2D();
            switch (data.friction)
            {
                case FoodFriction.Low: mat.friction = 0.1f; break;
                case FoodFriction.Medium: mat.friction = 0.4f; break;
                case FoodFriction.High: mat.friction = 0.8f; break;
            }
            col.sharedMaterial = mat;
        }
    }


}