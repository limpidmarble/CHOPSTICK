using UnityEngine;
using System.Collections.Generic;

public class ChopstickStick : MonoBehaviour
{
    public bool is_colliding = false;

    public bool is_touching_stick = false; // 이 젓가락이 다른 젓가락과 닿아있는지 여부입니다. true면 닿아있고, false면 닿아있지 않습니다.

    List<Collision2D> collisions = new List<Collision2D>(); //이 젓가락에 충돌한 물체들의 리스트입니다. 이 리스트에는 충돌이 시작되면 추가되고, 충돌이 끝나면 제거됨

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("충돌 감지: " + collision.gameObject.name + ", 태그: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Stick"))
        {
            Debug.Log("Collision with another chopstick detected.");
            is_touching_stick = true; // 다른 젓가락과 닿아있으면 true로 설정합니다.
        }
        else if (collision.gameObject.CompareTag("Target"))
        {
            collisions.Add(collision); // 충돌이 시작되면 collisions 리스트에 추가합니다.
        }
        is_colliding = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stick"))
        {
            is_touching_stick = false; // 다른 젓가락과 닿아있지 않으면 false로 설정합니다.
        }
        else if (collision.gameObject.CompareTag("Target"))
        {
            collisions.Remove(collision); // 충돌이 끝나면 collisions 리스트에서 제거합니다.
        }
        is_colliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
