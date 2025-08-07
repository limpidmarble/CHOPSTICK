using UnityEngine;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    public List<GameObject> typeAPrefabs;
    public List<GameObject> typeBPrefabs;
    public float spawnInterval = 5f;
    public int minFoodCount = 3;
    public Transform spawnPoint; // 하나만 사용

    public int foodInScene = 0;

    private float elapsedTime = 0f;

    // 대기열 관련
    public float dequeueInterval = 1f; // 대기열에서 스폰 간 최소 시간(초)
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    private float lastDequeueTime = -999f;

    public BoxCollider2D spawnCheckBox; // Inspector에서 직접 할당

    void Start()
    {
        for (int i = 0; i < 3; i++)
            EnqueueFoodByRule();

        InvokeRepeating(nameof(AutoEnqueue), spawnInterval, spawnInterval);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (foodInScene < minFoodCount)
            EnqueueFoodByRule();

        TrySpawnFromQueue();
    }

    GameObject GetRandomByRarity(List<GameObject> prefabs)
    {
        if (prefabs == null || prefabs.Count == 0) return null;

        // 희귀도별 가중치 (원하는 값으로 조정)
        Dictionary<FoodRarity, float> rarityWeights = new Dictionary<FoodRarity, float>
        {
            { FoodRarity.Common, 1f },
            { FoodRarity.Uncommon, 0.5f },
            { FoodRarity.Rare, 0.2f },
            { FoodRarity.Epic, 0.05f }
        };

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

        float rand = Random.value * totalWeight;
        float sum = 0f;
        for (int i = 0; i < prefabs.Count; i++)
        {
            sum += weights[i];
            if (rand <= sum)
                return prefabs[i];
        }
        return prefabs[prefabs.Count - 1];
    }

    void AutoEnqueue()
    {
        EnqueueFoodByRule();
    }

    void EnqueueFoodByRule()
    {
        float t = elapsedTime;
        GameObject prefabToEnqueue = null;
        float rand = Random.value;

        if (t <= 10f)
            prefabToEnqueue = GetRandomByRarity(typeAPrefabs);
        else if (t <= 25f)
            prefabToEnqueue = (rand < 0.9f) ? GetRandomByRarity(typeAPrefabs) : GetRandomByRarity(typeBPrefabs);
        else if (t <= 40f)
            prefabToEnqueue = (rand < 0.8f) ? GetRandomByRarity(typeAPrefabs) : GetRandomByRarity(typeBPrefabs);
        else
            prefabToEnqueue = (rand < 0.7f) ? GetRandomByRarity(typeAPrefabs) : GetRandomByRarity(typeBPrefabs);

        if (prefabToEnqueue != null)
            spawnQueue.Enqueue(prefabToEnqueue);
    }

    void TrySpawnFromQueue()
{
    if (spawnQueue.Count == 0) return;
    if (Time.time - lastDequeueTime < dequeueInterval) return;

    if (spawnCheckBox != null)
    {
        Vector2 boxSize = spawnCheckBox.size;
        Vector2 localRandom = new Vector2(
            Random.Range(-boxSize.x / 2f, boxSize.x / 2f),
            Random.Range(-boxSize.y / 2f, boxSize.y / 2f)
        );
        // 콜라이더의 로컬 랜덤 위치를 월드 좌표로 변환
        Vector2 worldRandom = spawnCheckBox.transform.TransformPoint(spawnCheckBox.offset + localRandom);

        // Target 태그 감지
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            spawnCheckBox.transform.TransformPoint(spawnCheckBox.offset),
            boxSize,
            spawnCheckBox.transform.eulerAngles.z
        );
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Target"))
                return;
        }

        Debug.Log($"Spawning food at: {worldRandom}");

        GameObject prefab = spawnQueue.Dequeue();
        foodInScene++;
        Instantiate(prefab, worldRandom, Quaternion.identity);
        lastDequeueTime = Time.time;
        return;
    }

    // fallback: spawnPoint 위치
    GameObject fallbackPrefab = spawnQueue.Dequeue();
    foodInScene++;
    Instantiate(fallbackPrefab, spawnPoint.position, Quaternion.identity);
    lastDequeueTime = Time.time;
}
}