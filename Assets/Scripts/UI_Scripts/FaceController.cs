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
        if (eatRoutine != null) return; // 먹는 중이면 얼굴 업데이트 금지

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
        if (eatRoutine != null) return; // 이미 애니메이션 중이면 무시
        eatRoutine = StartCoroutine(EatAnim());
    }

    IEnumerator EatAnim()
{
    // 더 빠르게: 입 벌림 → 씹기 → 입 다물기 → 씹기 → 원래 얼굴
    faceRenderer.sprite = face_eat_1;
    yield return new WaitForSeconds(0.25f);

    faceRenderer.sprite = face_eat_2;
    yield return new WaitForSeconds(0.33f);

    faceRenderer.sprite = face_eat_1;
    yield return new WaitForSeconds(0.25f);

    faceRenderer.sprite = face_eat_2;
    yield return new WaitForSeconds(0.33f);

    UpdateFace(GameManager.instance.CurrentFullness, GameManager.instance.maxFullness);
    eatRoutine = null;
}

    public void SetGameOverFace()
    {
        if (eatRoutine != null) StopCoroutine(eatRoutine);
        eatRoutine = null;
        faceRenderer.sprite = face_over;
    }
}