using UnityEngine;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab; // Food.cs가 붙은 프리팹(빈 오브젝트+SpriteRenderer+Collider2D)
    public FoodData[] foodDataList;
    public Vector2 spawnArea;
    public float spawnInterval = 3f;

    private List<FoodData> weightedList = new List<FoodData>();

    void Start()
    {
        BuildWeightedList();
        InvokeRepeating(nameof(SpawnFood), 1f, spawnInterval);
    }

    void BuildWeightedList()
    {
        weightedList.Clear();
        foreach (var data in foodDataList)
        {
            int weight = GetWeight(data.rarity);
            for (int i = 0; i < weight; i++)
                weightedList.Add(data);
        }
    }

    int GetWeight(FoodRarity rarity)
    {
        // C: 100, U: 50, R: 10, E: 1 (비율 유지)
        switch (rarity)
        {
            case FoodRarity.Common: return 100;
            case FoodRarity.Uncommon: return 50;
            case FoodRarity.Rare: return 10;
            case FoodRarity.Epic: return 1;
            default: return 1;
        }
    }

    void SpawnFood()
    {
        if (weightedList.Count == 0) return;
        int idx = Random.Range(0, weightedList.Count);
        FoodData data = weightedList[idx];

        float randomX = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float randomY = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0);

        GameObject foodObj = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        foodObj.GetComponent<Food>().data = data;
    }
}