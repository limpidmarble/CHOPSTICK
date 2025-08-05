using UnityEngine;
using System.Collections;

public class FaceController : MonoBehaviour
{
    public Sprite face_full;
    public Sprite face_usual;
    public Sprite face_hungry;
    public Sprite face_eat_1;
    public Sprite face_eat_2;
    public Sprite face_over;
    public SpriteRenderer faceRenderer; // 얼굴을 표시할 SpriteRenderer

    private Coroutine eatRoutine;

    public void UpdateFace(float fullness, float maxFullness)
    {
        float percent = fullness / maxFullness;
        if (percent >= 0.7f)
            faceRenderer.sprite = face_full;
        else if (percent >= 0.3f)
            faceRenderer.sprite = face_usual;
        else
            faceRenderer.sprite = face_hungry;
    }

    public void PlayEatAnimation()
    {
        if (eatRoutine != null) StopCoroutine(eatRoutine);
        eatRoutine = StartCoroutine(EatAnim());
    }

    IEnumerator EatAnim()
    {
        for (int i = 0; i < 3; i++)
        {
            faceRenderer.sprite = face_eat_1;
            yield return new WaitForSeconds(0.1f);
            faceRenderer.sprite = face_eat_2;
            yield return new WaitForSeconds(0.1f);
        }
        UpdateFace(GameManager.instance.CurrentFullness, GameManager.instance.maxFullness);
    }

    public void SetGameOverFace()
    {
        if (eatRoutine != null) StopCoroutine(eatRoutine);
        faceRenderer.sprite = face_over;
    }
}