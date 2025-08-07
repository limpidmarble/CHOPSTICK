using UnityEngine;
using System.Collections.Generic;

public class RigidChopstickStick : MonoBehaviour
{
    public bool is_colliding = false;

    public bool is_touching_stick = false; // 이 젓가락이 다른 젓가락과 닿아있는지 여부입니다. true면 닿아있고, false면 닿아있지 않습니다.

    PolygonCollider2D ghost_collider; //다른 물체들과 본체가 겹치는지 판단하는 콜라이더 (is_trigger되어 있음)

    PolygonCollider2D actual_collider; // 이 젓가락의 실제 콜라이더입니다. (이 콜라이더는 젓가락의 끝 부분에 위치합니다.)

    public Transform stick_target; // 젓가락이 있어야 할 목표 위치 오브젝트의 Transform을 저장하는 변수입니다.

    public Transform other_stick;

    PolygonCollider2D other_actual_collider; // 다른 젓가락의 실제 콜라이더입니다. (이 콜라이더는 젓가락의 끝 부분에 위치합니다.)

    public float initial_stick_relative_position; 
    public float stick_relative_position;
    public float stick_height;


    Vector2 offset;

    Rigidbody2D rb2d;

    public Dictionary<GameObject, ContactPoint2D> collidingObjects = new Dictionary<GameObject, ContactPoint2D>{}; //이 젓가락에 충돌한 물체들의 리스트입니다. 이 리스트에는 충돌이 시작되면 추가되고, 충돌이 끝나면 제거됨

    public float return_speed = 2000f; //원래 위치로 돌아가는 속도입니다.
    public Vector3 velocity = Vector3.zero;

    public float vertical_force; // 일종의 젓가락의 수직 방향 오프셋? 꽃히는 로직 구현할 때 사용. 실제로 움직일 때 속도 값이기도 하고 물체에 대고 커서를 물체 쪽으로 가까이 할 때도 절댓값이 늘어남.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        return_speed = 1000f; // 초기 속도를 설정합니다.


        rb2d = GetComponent<Rigidbody2D>();

        stick_height = GetComponent<SpriteRenderer>().bounds.size.y; // 젓가락의 높이를 Collider2D의 bounds에서 가져옵니다.

        initial_stick_relative_position = transform.position.x - other_stick.position.x; // 다른 젓가락과의 상대 위치를 계산합니다. (왼쪽 젓가락은 음수, 오른쪽 젓가락은 양수로 설정됩니다.)

        PolygonCollider2D[] colliders = gameObject.GetComponents<PolygonCollider2D>();
        ghost_collider = colliders[0]; // 첫 번째 콜라이더는 트리거로 설정되어 있습니다.
        actual_collider = colliders[1];
        PolygonCollider2D[] other_colliders = other_stick.GetComponents<PolygonCollider2D>();
        other_actual_collider = other_colliders[1];

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
            MonoBehaviour target_script = collision.gameObject.GetComponent<MonoBehaviour>();
            collidingObjects[collision.gameObject] = collision.contacts[0]; // 충돌한 물체와의 접촉점을 저장합니다.
            if (target_script != null)
            {

            }
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

    void OnTriggerEnter2D(Collider2D collision)
    {
    }

    void OnTriggerStay2D(Collider2D collision)
    {
    }

    void OnTriggerExit2D(Collider2D collision)
    {
    }

    // Update is called once per frame
    public Vector3 ToTargetPosition(float target_angle)
    {
        stick_relative_position = transform.position.x - other_stick.position.x; // 현재 젓가락과 다른 젓가락의 상대 위치를 계산합니다. (왼쪽 젓가락은 음수, 오른쪽 젓가락은 양수가 일반적인 상황)
        if (initial_stick_relative_position * stick_relative_position < 0) // 현재 젓가락이 왼쪽에 있고, 다른 젓가락이 오른쪽에 있는 경우
        {
            Debug.Log("충돌 무시: " + other_stick.name);
            Physics2D.IgnoreCollision(other_actual_collider, actual_collider); // 충돌을 무시합니다.
        }
        else
        {
            Physics2D.IgnoreCollision(other_actual_collider, actual_collider, false); // 충돌을 무시하지 않습니다.

        }
        Vector3 target_position = stick_target.position; // 돌아가는 목표 위치입니다.
        Vector3 direction = target_position - transform.position;
        velocity = direction * return_speed * Time.fixedDeltaTime;
        float bef_x = transform.position.x - target_position.x;
        float bef_y = transform.position.y - target_position.y;

        vertical_force = transform.InverseTransformDirection(velocity).y;

        rb2d.MoveRotation(target_angle); // 목표 앵글로 회전합니다.

        if (bef_x > 0.01f || bef_x < -0.01f || bef_y > 0.01f || bef_y < -0.01f) // 목표 위치와 현재 위치의 차이가 0.01보다 크면
        {
            rb2d.linearVelocity = velocity; // 속도를 설정합니다.
            return velocity; // 현재 속도를 반환합니다.
        }
        else
        {
            rb2d.linearVelocity = Vector2.zero; // 목표 위치에 도달하면 속도를 0으로 설정합니다.
            return Vector3.zero;
        }
    }
}
