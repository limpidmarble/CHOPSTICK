using UnityEngine;

public class Food : MonoBehaviour
{
    public FoodData data; // FoodData 스크립트able object를 참조

    private void Start()
{
    // Sprite 적용
    GetComponent<SpriteRenderer>().sprite = data.sprite;
    // 질량 적용
    GetComponent<Rigidbody2D>().mass = Mathf.Ceil(data.mass * 0.5f);

    // 마찰 및 탄성 적용
    var col = GetComponent<Collider2D>();
    if (col != null)
    {
        PhysicsMaterial2D mat = new PhysicsMaterial2D();
        // 마찰
        switch (data.friction)
        {
            case FoodFriction.Low: mat.friction = 0.1f; break;
            case FoodFriction.Medium: mat.friction = 0.4f; break;
            case FoodFriction.High: mat.friction = 0.8f; break;
        }
        // 탄성
        switch (data.bounciness)
        {
            case FoodBounciness.None:   mat.bounciness = 0f; break;
            case FoodBounciness.Low:    mat.bounciness = 0.2f; break;
            case FoodBounciness.Medium: mat.bounciness = 0.5f; break;
            case FoodBounciness.High:   mat.bounciness = 0.7f; break;
        }
        col.sharedMaterial = mat;
    }
}
}
