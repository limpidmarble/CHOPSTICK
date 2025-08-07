using UnityEngine;

public class TitleStartArea : MonoBehaviour
{
    public TitleInteractable title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (title.canStart)
        {
            if (collision.CompareTag("IButton"))
            {
            title.InitiateStart();
            }
        }
        
    }
}
