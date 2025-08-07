using UnityEngine;

public class TitleInteractable : MonoBehaviour
{
    public Sprite fullSceneSprite;
    public Sprite holeSceneSprite;

    private SpriteRenderer spriteRenderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void ChangeSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }
}
