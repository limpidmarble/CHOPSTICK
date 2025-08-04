using UnityEngine;
using System.Collections.Generic;

public class ChopstickStick : MonoBehaviour
{
    public bool is_colliding = false;

    public bool is_touching_stick = false; // 이 젓가락이 다른 젓가락과 닿아있는지 여부입니다. true면 닿아있고, false면 닿아있지 않습니다.

    List<Collision2D> collisions = new List<Collision2D>(); //이 젓가락에 충돌한 물체들의 리스트입니다. 이 리스트에는 충돌이 시작되면 추가되고, 충돌이 끝나면 제거됨

    public float return_speed = 5f; //원래 위치로 돌아가는 속도입니다.
    Vector3 velocity = Vector3.zero;

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

    public void ReturnToOriginalPosition(float axis_height)
    {
        Vector3 target_position = new Vector3(0f, -axis_height, 0f); // 돌아가는 목표 위치입니다.
        Vector3 direction = target_position - transform.localPosition;
        velocity = direction.normalized * return_speed * Time.deltaTime;
        float bef_x = transform.localPosition.x - target_position.x;
        float bef_y = transform.localPosition.y - target_position.y;
        transform.localPosition += velocity; // 현재 위치에서 목표 위치로 이동합니다.
        float aft_x = transform.localPosition.x - target_position.x;
        float aft_y = transform.localPosition.y - target_position.y;
        if (bef_x * aft_x < 0 || bef_y * aft_y < 0) // 만약 이동 후 위치가 목표 위치를 지나쳤다면
        {
            transform.localPosition = target_position; // 목표 위치로 고정합니다.
        }
    }
}
