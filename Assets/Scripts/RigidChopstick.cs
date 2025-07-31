using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidChopstick : MonoBehaviour
{
    public float stick_squeeze_angle_left; // 제일 기본적인 젓가락 변수입니다. 0도 (11자 상태) 에서 얼마나 기울어져있는지 표시합니다. (단위: 도).물체와 충돌해 젓가락 자체는 멈춰도 stick_squeeze_angle은 최대치까지 올라갈 수 있습니다.
                                           // 최대치는 90 정도가 적당할 것 같습니다.
    public float stick_squeeze_angle_right;
    public float stick_squeeze_min_angle = -3f; // 최소 앵글이자 기본 앵글 값입니다.
    float stick_squeeze_max_angle = 90f; // 최대 앵글 값입니다.
    public float stick_squeeze_speed = 20f; // 젓가락을 오므리는 속도입니다. 단위는 도/초입니다. 90f는 1초에 90도 회전합니다.                          
    public float angle_on_collision_left; // 양 젓가락이 모두 물체와 닿아 더 이상 기울어질 수 없을 때의 앵글 값을 저장하는 변수입니다. 이 변수값과, 더 올라간 stick_squeeze_angle값과의 차이를 구해 힘값을 도출합니다.
    public float angle_on_collision_right;
    public bool is_locked = false; // 젓가락이 서로 맞닿거나 물체와 충돌해 더 이상 움직이지 못할 때 true로 설정됩니다.(물체와 충돌하지 않으면 false로 돌아갑니다.)

    Transform stick_left_axis; // 왼쪽 젓가락 축의 Transform을 저장하는 변수입니다.
    Transform stick_right_axis; // 오른쪽 젓가락 축의 Transform을 저장하는 변수입니다.
    Transform left_target; // 이 변수는 ChopstickLeftTarget의 RigidChopstickTarget 컴포넌트를 참조합니다.
    Transform right_target; // 이 변수는 ChopstickRightTarget의 RigidChopstickTarget 컴포넌트를 참조합니다.
    public RigidChopstickStick left_stick; // 이 변수는 ChopstickLeft의 RigidChopstickStick 컴포넌트를 참조합니다.
    public RigidChopstickStick right_stick; // 이 변수는 ChopstickRight의 RigidChopstickStick 컴포넌트를 참조합니다.

    float main_axis_height = 1f; //왼/오 젓가락 스프라이트의 길이 중간점을 기준으로 한 축 높이입니다. 실제 젓가락을 잡을 때 중간점보다는 살짝 위로 잡으므로 기본값은 1입니다. (= 각 스프라이트는 각 축의 자식이므로 왼/오 젓가락의 y position은 -1로 나타남)
    float main_axis_width = 2f; //왼/오 젓가락 스프라이트의 너비입니다. 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60; // 최대 60프레임으로 제한
        stick_left_axis = transform.Find("ChopstickLeftAxis"); // ChopstickLeftAxis는 왼쪽 젓가락의 회전축 역할을 하는 오브젝트입니다. 축이 각 젓가락의 부모이기 때문에 이걸 회전시키면 대응하는 젓가락 오브젝트도 같이 돌아갑니다.
        stick_right_axis = transform.Find("ChopstickRightAxis"); //이하 동일
        left_target = transform.Find("ChopstickLeftAxis/ChopstickLeftTarget"); // 왼쪽 젓가락 타겟 오브젝트의 RigidChopstickTarget 컴포넌트를 가져옵니다.
        right_target = transform.Find("ChopstickRightAxis/ChopstickRightTarget"); // 오른쪽 젓가락 타겟 오브젝트의 RigidChopstickTarget 컴포넌트를 가져옵니다.

        stick_squeeze_angle_left = stick_squeeze_min_angle; // 젓가락의 기본 앵글은 stick_squeeze_min_angle로 설정
        stick_squeeze_angle_right = stick_squeeze_min_angle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mouse_screen = Input.mousePosition;
        Vector3 mouse_world = Camera.main.ScreenToWorldPoint(mouse_screen);
        transform.position = new Vector3(mouse_world.x, mouse_world.y, 0f);

        stick_left_axis.localPosition = new Vector3(-main_axis_width / 2f, -main_axis_height, 0f); // 왼쪽 젓가락 축의 위치를 설정합니다. 왼쪽 젓가락은 왼쪽으로 이동해야 하므로 x좌표는 -main_axis_width / 2f로 설정합니다.
        stick_right_axis.localPosition = new Vector3(main_axis_width / 2f, -main_axis_height, 0f); // 오른쪽 젓가락 축의 위치를 설정합니다. 오른쪽 젓가락은 오른쪽으로 이동해야 하므로 x좌표는 main_axis_width / 2f로 설정합니다.


        if (Input.GetMouseButton(0))
        {
            stick_squeeze_angle_left += stick_squeeze_speed * Time.deltaTime; // 마우스 왼쪽 버튼을 누르고 있으면 stick_squeeze_angle이 초당 stick_squeeze_speed만큼 증가합니다. 
            if (stick_squeeze_angle_left > stick_squeeze_max_angle) // stick_squeeze_angle이 최대를 넘어가면
            {
                stick_squeeze_angle_left = stick_squeeze_max_angle; // 최대로 고정합니다.
            }
        }
        else
        {
            if (stick_squeeze_angle_left > stick_squeeze_min_angle) //stick_squeeze_angle이 최소보다 크면
                stick_squeeze_angle_left -= stick_squeeze_speed * Time.deltaTime; // 마우스 왼쪽 버튼을 누르고 있지 않으면 stick_squeeze_angle이 초당 stick_squeeze_speed 만큼 감소합니다.
            if (stick_squeeze_angle_left < stick_squeeze_min_angle) // stick_squeeze_angle이 최소보다 작아지면
            {
                stick_squeeze_angle_left = stick_squeeze_min_angle; // 최솟값으로 고정합니다.
            }
        }

        if (Input.GetMouseButton(1))
        {
            stick_squeeze_angle_right += stick_squeeze_speed * Time.deltaTime; // 마우스 왼쪽 버튼을 누르고 있으면 stick_squeeze_angle이 초당 stick_squeeze_speed만큼 증가합니다. 
            if (stick_squeeze_angle_right > stick_squeeze_max_angle) // stick_squeeze_angle이 최대를 넘어가면
            {
                stick_squeeze_angle_right = stick_squeeze_max_angle; // 최대로 고정합니다.
            }
        }
        else
        {
            if (stick_squeeze_angle_right > stick_squeeze_min_angle) //stick_squeeze_angle이 최소보다 크면
                stick_squeeze_angle_right -= stick_squeeze_speed * Time.deltaTime; // 마우스 왼쪽 버튼을 누르고 있지 않으면 stick_squeeze_angle이 초당 stick_squeeze_speed 만큼 감소합니다.
            if (stick_squeeze_angle_right < stick_squeeze_min_angle) // stick_squeeze_angle이 최소보다 작아지면
            {
                stick_squeeze_angle_right = stick_squeeze_min_angle; // 최솟값으로 고정합니다.
            }
        }

        if (!is_locked)
        {
            if (left_stick.is_touching_stick && right_stick.is_touching_stick) // 왼쪽, 오른쪽 젓가락이 닿아있으면
            {
                is_locked = true; // 젓가락이 서로 맞닿아 더 이상 움직이지 못하는 상태로 설정합니다.
                angle_on_collision_left = stick_squeeze_angle_left; // 현재 stick_squeeze_angle을 angle_on_collision에 저장합니다.
                angle_on_collision_right = stick_squeeze_angle_right; // 현재 stick_squeeze_angle을 angle_on_collision에 저장합니다.
            }
            stick_left_axis.localRotation = Quaternion.Euler(0, 0, stick_squeeze_angle_left); // 왼쪽 젓가락 축의 회전값을 stick_squeeze_angle로 설정합니다.
            stick_right_axis.localRotation = Quaternion.Euler(0, 0, -stick_squeeze_angle_right); // 오른쪽 젓가락 축의 회전값을 -stick_squeeze_angle로 설정합니다. -인 이유는 오른쪽 젓가락이 왼쪽 젓가락과 반대 방향으로 회전해야 오므려지기 때문입니다.

            left_stick.ToTargetPosition(stick_squeeze_angle_left);
            right_stick.ToTargetPosition(-stick_squeeze_angle_right); // 왼쪽, 오른쪽 젓가락을 목표 위치로 이동시키는 함수 호출
        }
        else
        {
            if (angle_on_collision_left == stick_squeeze_min_angle && angle_on_collision_right == stick_squeeze_min_angle) // 만약 angle_on_collision이 stick_squeeze_min_angle과 같다면
            {
                is_locked = false; // is_locked를 false로 설정합니다. 즉, 물체와 충돌하지 않으면 젓가락은 다시 움직일 수 있습니다.
                angle_on_collision_left = 0f; // angle_on_collision을 0으로 초기화합니다.
                angle_on_collision_right = 0f; // angle_on_collision을 0으로 초기화합니다.
            }
            if (stick_squeeze_angle_left <= angle_on_collision_left)
            {
                is_locked = false; // 만약 stick_squeeze_angle이 angle_on_collision보다 작아지면 is_locked를 false로 설정합니다. 즉, 물체와 충돌하지 않으면 젓가락은 다시 움직일 수 있습니다.
            }
            if (stick_squeeze_angle_right <= angle_on_collision_right)
            {
                is_locked = false; // 만약 stick_squeeze_angle이 angle_on_collision보다 작아지면 is_locked를 false로 설정합니다. 즉, 물체와 충돌하지 않으면 젓가락은 다시 움직일 수 있습니다.
            }

            left_stick.ToTargetPosition(angle_on_collision_left);
            right_stick.ToTargetPosition(-angle_on_collision_right); // 왼쪽, 오른쪽 젓가락을 목표 위치로 이동시키는 함수 호출
        }

    }
}
