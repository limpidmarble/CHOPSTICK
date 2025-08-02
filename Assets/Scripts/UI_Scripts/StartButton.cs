using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AudioClip clickSound; // 인스펙터에서 할당
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

   public void StartButton()
{
    if (clickSound != null)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clickSound;
        tempSource.Play();
        DontDestroyOnLoad(tempAudio);
        Destroy(tempAudio, clickSound.length);
    }
    SceneManager.LoadScene("SampleScene");
}
}