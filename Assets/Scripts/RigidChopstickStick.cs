using UnityEngine;
using System.Collections.Generic;

public class RigidChopstickStick : MonoBehaviour
{
    public bool is_colliding = false;

    public bool is_touching_stick = false; // 이 젓가락이 다른 젓가락과 닿아있는지 여부입니다. true면 닿아있고, false면 닿아있지 않습니다.

    public Transform stick_target; // 젓가락이 있어야 할 목표 위치 오브젝트의 Transform을 저장하는 변수입니다.

    public float stick_height;

    Vector2 offset;

    Rigidbody2D rb2d;

    List<GameObject> collidingObjects = new List<GameObject>(); //이 젓가락에 충돌한 물체들의 리스트입니다. 이 리스트에는 충돌이 시작되면 추가되고, 충돌이 끝나면 제거됨

    public float return_speed = 2000f; //원래 위치로 돌아가는 속도입니다.
    Vector3 velocity = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        return_speed = 1000f; // 초기 속도를 설정합니다.


        rb2d = GetComponent<Rigidbody2D>();

        stick_height = GetComponent<Collider2D>().bounds.size.y; // 젓가락의 높이를 Collider2D의 bounds에서 가져옵니다.

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("충돌 감지: " + collision.gameObject.name + ", 태그: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Stick"))
        {
            is_touching_stick = true; // 다른 젓가락과 닿아있으면 true로 설정합니다.
        }
        else if (collision.gameObject.CompareTag("Target"))
        {
            if (!collidingObjects.Contains(collision.gameObject))
            collidingObjects.Add(collision.gameObject);
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
            collidingObjects.Remove(collision.gameObject);
        }
        is_colliding = false;

        Debug.Log("충돌 종료: " + collision.gameObject.name + ", 태그: " + collision.gameObject.tag);
    }
    // Update is called once per frame
    public void ToTargetPosition(float target_angle)
    {
        Vector3 target_position = stick_target.position; // 돌아가는 목표 위치입니다.
        Vector3 direction = target_position - transform.position;
        velocity = direction * return_speed * Time.fixedDeltaTime;
        float bef_x = transform.position.x - target_position.x;
        float bef_y = transform.position.y - target_position.y;

        rb2d.MoveRotation(target_angle); // 목표 앵글로 회전합니다.

        if (bef_x > 0.01f || bef_x < -0.01f || bef_y > 0.01f || bef_y < -0.01f) // 목표 위치와 현재 위치의 차이가 0.01보다 크면
        {
            rb2d.linearVelocity = velocity; // 속도를 설정합니다.
        }
        else
        {
            rb2d.linearVelocity = Vector2.zero; // 목표 위치에 도달하면 속도를 0으로 설정합니다.
        }
    }
}
