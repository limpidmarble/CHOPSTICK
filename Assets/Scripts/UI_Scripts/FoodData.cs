using UnityEngine;

[CreateAssetMenu(menuName = "Food/FoodData")]
public class FoodData : ScriptableObject
{
    public string foodName;
    public Sprite sprite;
    public int score;
    public float mass;
    public float fullnessValue;
    public FoodRarity rarity;
    public FoodFriction friction;
    public FoodBounciness bounciness;
}

public enum FoodRarity { Common, Uncommon, Rare, Epic }
public enum FoodFriction { Low, Medium, High }
public enum FoodBounciness { None, Low, Medium, High }
