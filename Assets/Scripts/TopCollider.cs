using UnityEngine;

public class TopCollider : MonoBehaviour
{
    // 뿅 사라지는 이펙트(파티클) 프리팹을 연결할 변수
    public GameObject successEffect;

    // 뿅 하는 사운드를 연결할 변수
    public AudioClip successSound;

    // 사운드를 재생할 오디오 소스 컴포넌트
    private AudioSource audioSource;

    void Start()
    {
        // 이 오브젝트에 붙어있는 AudioSource 컴포넌트를 가져옴
        // 만약 없다면 하나 새로 추가함
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 다른 콜라이더가 트리거 영역으로 들어왔을 때 호출되는 함수
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 오브젝트의 태그가 "Target"이라면 (음식이라면)
        if (other.CompareTag("Target"))
        {
            // GameManager를 찾아서 공복도를 감소시키는 함수를 호출
            GameManager.instance.IncreaseFullness(10);

            // 뿅! 하는 이펙트 생성
            if (successEffect != null)
            {
                // other.transform.position은 음식이 있던 바로 그 위치
                Instantiate(successEffect, other.transform.position, Quaternion.identity);
            }

            // 뿅! 하는 사운드 재생
            if (successSound != null)
            {
                // PlayOneShot은 여러 소리가 겹쳐서 재생될 수 있게 해줌
                audioSource.PlayOneShot(successSound);
            }

            // 음식 오브젝트를 파괴하여 사라지게 함
            Destroy(other.gameObject);
        }
    }
}
