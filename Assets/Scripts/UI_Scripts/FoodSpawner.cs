using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // 1. 여러 개의 음식 프리팹을 담을 '배열' 변수
    public GameObject[] foodPrefabs;

    // 2. 음식이 생성될 범위를 정하는 변수 (가로, 세로)
    public Vector2 spawnArea;

    // 3. 생성 주기 (초 단위)
    public float spawnInterval = 3f;

    void Start()
    {
        InvokeRepeating("SpawnFood", 1f, spawnInterval);
    }

    void SpawnFood()
    {
        // foodPrefabs 배열이 비어있으면 에러 메시지를 띄우고 함수를 종료
        if (foodPrefabs.Length == 0)
        {
            Debug.LogError("Food Prefabs 배열이 비어있습니다. Inspector에서 프리팹을 할당해주세요.");
            return;
        }

        // --- 핵심 변경 부분 ---
        // 0부터 배열의 길이 -1 사이의 정수 중 하나를 랜덤으로 뽑음
        int randomIndex = Random.Range(0, foodPrefabs.Length);

        // 랜덤 인덱스를 이용해 배열에서 생성할 음식 프리팹을 선택
        GameObject foodToSpawn = foodPrefabs[randomIndex];
        // --- 여기까지 ---

        // 스포너의 위치를 기준으로 랜덤한 좌표를 계산
        float randomX = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float randomY = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0);

        // 위에서 랜덤으로 선택한 foodToSpawn을 생성
        Instantiate(foodToSpawn, spawnPosition, Quaternion.identity);
    }
}