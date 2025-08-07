using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public AudioSource audioSource;

    public AudioClip slide_in;
    public AudioClip short_tap;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴 방지
        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 중복 생성 방지
        }
    }

    public void SlideIn()
    {
        audioSource.PlayOneShot(slide_in);
    }

    public void StickTap()
    {
        audioSource.PlayOneShot(short_tap);
    }
}
    
