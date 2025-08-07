using UnityEngine;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    public List<GameObject> foodPrefabs; // Inspector에서 모든 프리팹 할당

    public float spawnInterval = 5f;
    public int minFoodCount = 3;
    public Transform spawnPoint;

    public int foodInScene = 0;

    private float elapsedTime = 0f;

    // 대기열 관련
    public float dequeueInterval = 1f;
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    private float lastDequeueTime = -999f;

    public BoxCollider2D spawnCheckBox;

    // 등급별 리스트
    private List<GameObject> commonPrefabs = new List<GameObject>();
    private List<GameObject> uncommonPrefabs = new List<GameObject>();
    private List<GameObject> rarePrefabs = new List<GameObject>();
    private List<GameObject> epicPrefabs = new List<GameObject>();

    void Start()
    {
        // 등급별로 분류
        foreach (var prefab in foodPrefabs)
        {
            var food = prefab.GetComponent<Food>();
            if (food == null || food.data == null) continue;
            switch (food.data.rarity)
            {
                case FoodRarity.Common:   commonPrefabs.Add(prefab); break;
                case FoodRarity.Uncommon: uncommonPrefabs.Add(prefab); break;
                case FoodRarity.Rare:     rarePrefabs.Add(prefab); break;
                case FoodRarity.Epic:     epicPrefabs.Add(prefab); break;
            }
        }

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

    // 1분(60초) 동안 선형적으로 희귀도별 가중치 변화
    Dictionary<FoodRarity, float> GetRarityWeights()
    {
        float t = Mathf.Clamp(elapsedTime, 0f, 60f) / 60f;
        return new Dictionary<FoodRarity, float>
        {
            { FoodRarity.Common,   Mathf.Lerp(1f,   0.8f, t) },
            { FoodRarity.Uncommon, Mathf.Lerp(0.5f, 0.3f, t) },
            { FoodRarity.Rare,     Mathf.Lerp(0.2f, 0.3f, t) },
            { FoodRarity.Epic,     Mathf.Lerp(0f,   0.1f, t) }
        };
    }

    GameObject GetRandomByRarity()
    {
        var rarityWeights = GetRarityWeights();

        // 실제 존재하는 등급만 추림
        List<(FoodRarity rarity, List<GameObject> list)> available = new();
        float totalWeight = 0f;
        if (commonPrefabs.Count > 0)   { available.Add((FoodRarity.Common, commonPrefabs));   totalWeight += rarityWeights[FoodRarity.Common]; }
        if (uncommonPrefabs.Count > 0) { available.Add((FoodRarity.Uncommon, uncommonPrefabs)); totalWeight += rarityWeights[FoodRarity.Uncommon]; }
        if (rarePrefabs.Count > 0)     { available.Add((FoodRarity.Rare, rarePrefabs));       totalWeight += rarityWeights[FoodRarity.Rare]; }
        if (epicPrefabs.Count > 0)     { available.Add((FoodRarity.Epic, epicPrefabs));       totalWeight += rarityWeights[FoodRarity.Epic]; }

        if (available.Count == 0) return null;

        // 등급 뽑기
        float rand = Random.value * totalWeight;
        float sum = 0f;
        List<GameObject> chosenList = available[0].list;
        for (int i = 0; i < available.Count; i++)
        {
            sum += rarityWeights[available[i].rarity];
            if (rand <= sum)
            {
                chosenList = available[i].list;
                break;
            }
        }

        // 해당 등급 내에서 랜덤 선택
        return chosenList[Random.Range(0, chosenList.Count)];
    }

    void AutoEnqueue()
    {
        EnqueueFoodByRule();
    }

    void EnqueueFoodByRule()
    {
        GameObject prefabToEnqueue = GetRandomByRarity();
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