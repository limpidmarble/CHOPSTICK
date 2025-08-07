using UnityEngine;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    public List<GameObject> typeAPrefabs;
    public List<GameObject> typeBPrefabs;
    public Vector2 spawnArea;
    public float spawnInterval = 5f;
    public int minFoodCount = 3;

    public int foodInScene = 0;

    private float elapsedTime = 0f;

    void Start()
    {
        for (int i = 0; i < 3; i++)
            SpawnFoodByRule();

        InvokeRepeating(nameof(AutoSpawn), spawnInterval, spawnInterval);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (foodInScene < minFoodCount)
            SpawnFoodByRule();
    }

    void AutoSpawn()
    {
        SpawnFoodByRule();
    }

    void SpawnFoodByRule()
    {
        float t = elapsedTime;
        GameObject prefabToSpawn = null;
        float rand = Random.value;

        if (t <= 45f)
        {
            prefabToSpawn = GetRandomByRarity(typeAPrefabs);
        }
        else if (t <= 90f)
        {
            prefabToSpawn = (rand < 0.95f) ? GetRandomByRarity(typeAPrefabs) : GetRandomByRarity(typeBPrefabs);
        }
        else if (t <= 150f)
        {
            prefabToSpawn = (rand < 0.9f) ? GetRandomByRarity(typeAPrefabs) : GetRandomByRarity(typeBPrefabs);
        }
        else
        {
            prefabToSpawn = (rand < 0.8f) ? GetRandomByRarity(typeAPrefabs) : GetRandomByRarity(typeBPrefabs);
        }

        if (prefabToSpawn == null) return;

        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
            0);

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        foodInScene++;
    }

    GameObject GetRandomByRarity(List<GameObject> prefabs)
    {
        if (prefabs == null || prefabs.Count == 0) return null;

        // 희귀도별 가중치 값 (원하는대로 조정)
        Dictionary<FoodRarity, float> rarityWeights = new Dictionary<FoodRarity, float>
        {
            { FoodRarity.Common, 1f },
            { FoodRarity.Uncommon, 0.5f },
            { FoodRarity.Rare, 0.2f },
            { FoodRarity.Epic, 0.05f }
        };

        // 전체 가중치 합 계산
        float totalWeight = 0f;
        List<float> weights = new List<float>();
        foreach (var prefab in prefabs)
        {
            var food = prefab.GetComponent<Food>();
            if (food == null || food.data == null) { weights.Add(0); continue; }
            float w = rarityWeights.ContainsKey(food.data.rarity) ? rarityWeights[food.data.rarity] : 1f;
            weights.Add(w);
            totalWeight += w;
        }

        // 가중치 랜덤 추출
        float rand = Random.value * totalWeight;
        float sum = 0f;
        for (int i = 0; i < prefabs.Count; i++)
        {
            sum += weights[i];
            if (rand <= sum)
                return prefabs[i];
        }
        return prefabs[prefabs.Count - 1]; // fallback
    }
}