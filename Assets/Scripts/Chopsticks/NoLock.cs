using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoLock : MonoBehaviour
{
    public float stick_squeeze_angle_left; // 제일 기본적인 젓가락 변수입니다. 0도 (11자 상태) 에서 얼마나 기울어져있는지 표시합니다. (단위: 도).물체와 충돌해 젓가락 자체는 멈춰도 stick_squeeze_angle은 최대치까지 올라갈 수 있습니다.
                                           // 최대치는 90 정도가 적당할 것 같습니다.
    public float stick_squeeze_angle_right;
    public float stick_squeeze_min_angle = -3f; // 최소 앵글이자 기본 앵글 값입니다.
    float stick_squeeze_max_angle = 90f; // 최대 앵글 값입니다.
    public float stick_squeeze_speed = 50f; // 젓가락을 오므리는 속도입니다. 단위는 도/초입니다. 90f는 1초에 90도 회전합니다.                          
    public float angle_on_collision_left; // 양 젓가락이 모두 물체와 닿아 더 이상 기울어질 수 없을 때의 앵글 값을 저장하는 변수입니다. 이 변수값과, 더 올라간 stick_squeeze_angle값과의 차이를 구해 힘값을 도출합니다.
    public float angle_on_collision_right;
    public bool is_locked = false; // 젓가락이 서로 맞닿거나 물체와 충돌해 더 이상 움직이지 못할 때 true로 설정됩니다.(물체와 충돌하지 않으면 false로 돌아갑니다.)
    public bool is_locked_by_target = false; // 젓가락이 타겟과 충돌해 더 이상 움직이지 못할 때 true로 설정됩니다. (물체와 충돌하지 않으면 false로 돌아갑니다.)

    public List<GameObject> holdingTargets = new List<GameObject>(); // 젓가락이 잡고 있는 타겟 오브젝트들의 리스트입니다. 이 리스트에는 젓가락이 타겟을 잡으면 추가되고, 타겟을 놓으면 제거됩니다.

    Transform stick_left_axis; // 왼쪽 젓가락 축의 Transform을 저장하는 변수입니다.s
    Transform stick_right_axis; // 오른쪽 젓가락 축의 Transform을 저장하는 변수입니다.
    Transform left_target; // 이 변수는 ChopstickLeftTarget의 RigidChopstickTarget 컴포넌트를 참조합니다.
    Transform right_target; // 이 변수는 ChopstickRightTarget의 RigidChopstickTarget 컴포넌트를 참조합니다.
    public RigidChopstickStick left_stick; // 이 변수는 ChopstickLeft의 RigidChopstickStick 컴포넌트를 참조합니다.
    public RigidChopstickStick right_stick; // 이 변수는 ChopstickRight의 RigidChopstickStick 컴포넌트를 참조합니다.

    Vector3 left_stick_velocity; // 왼쪽 젓가락의 속도를 저장하는 변수입니다.
    Vector3 right_stick_velocity;

    Vector3 stick_average_velocity; // 왼쪽, 오른쪽 젓가락의 평균 속도를 저장하는 변수입니다.

    public float main_axis_height = 1f; //왼/오 젓가락 스프라이트의 길이 중간점을 기준으로 한 축 높이입니다. 실제 젓가락을 잡을 때 중간점보다는 살짝 위로 잡으므로 기본값은 1입니다. (= 각 스프라이트는 각 축의 자식이므로 왼/오 젓가락의 y position은 -1로 나타남)

    float main_axis_height_max = 2f; //왼/오 젓가락 스프라이트의 길이 중간점을 기준으로 한 축 높이의 최대값입니다.

    float main_axis_height_min = -2f; //왼/오 젓가락 스프라이트의 길이 중간점을 기준으로 한 축 높이의 최소값입니다.

    public float main_axis_height_change_speed = 0.1f; //왼/오 젓가락 스프라이트의 길이 중간점을 기준으로 한 축 높이를 변경하는 속도입니다. (단위: 초당 변화량)

    public float main_axis_width = 2f; //왼/오 젓가락 스프라이트의 너비입니다. 

    public float main_axis_width_max = 4f; //왼/오 젓가락 스프라이트의 너비의 최대값입니다. (왼쪽 젓가락은 왼쪽으로, 오른쪽 젓가락은 오른쪽으로 이동해야 하므로 양수로 설정합니다.)

    public float main_axis_width_min = 2f;

    public float main_axis_width_change_speed = 0.1f; //왼/오 젓가락 스프라이트의 너비를 변경하는 속도입니다. (단위: 초당 변화량)

    bool axis_width_increased = false; // 왼/오 젓가락 스프라이트의 너비가 증가했는지 여부를 나타내는 변수입니다. true면 증가했고, false면 감소했습니다.

    bool axis_width_decreased = false; // 왼/오 젓가락 스프라이트의 너비가 감소했는지 여부를 나타내는 변수입니다. true면 감소했고, false면 증가했습니다.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60; // 최대 60프레임으로 제한

        Cursor.visible = false;
                
        stick_left_axis = transform.Find("ChopstickLeftAxis"); // ChopstickLeftAxis는 왼쪽 젓가락의 회전축 역할을 하는 오브젝트입니다. 축이 각 젓가락의 부모이기 때문에 이걸 회전시키면 대응하는 젓가락 오브젝트도 같이 돌아갑니다.
        stick_right_axis = transform.Find("ChopstickRightAxis"); //이하 동일
        left_target = transform.Find("ChopstickLeftAxis/ChopstickLeftTarget"); // 왼쪽 젓가락 타겟 오브젝트의 RigidChopstickTarget 컴포넌트를 가져옵니다.
        right_target = transform.Find("ChopstickRightAxis/ChopstickRightTarget"); // 오른쪽 젓가락 타겟 오브젝트의 RigidChopstickTarget 컴포넌트를 가져옵니다.

        main_axis_height_max = left_stick.stick_height / 1f; // 왼쪽 젓가락의 높이의 절반을 main_axis_height_max로 설정합니다. (왼쪽 젓가락의 높이는 오른쪽 젓가락과 동일하므로 왼쪽 젓가락의 높이를 사용합니다.)
        main_axis_height_min = -left_stick.stick_height / 1f; // same logic as above, but for minimum height (AI가 왜 영어랑 한국어를 섞어서 쓰는지?)

        stick_squeeze_angle_left = stick_squeeze_min_angle; // 젓가락의 기본 앵글은 stick_squeeze_min_angle로 설정
        stick_squeeze_angle_right = stick_squeeze_min_angle;

        main_axis_height_change_speed = 0.2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mouse_screen = Input.mousePosition;
        Vector3 mouse_world = Camera.main.ScreenToWorldPoint(mouse_screen);
        transform.position = new Vector3(mouse_world.x, mouse_world.y, 0f);


        if (Input.GetMouseButton(0))
        {
            stick_squeeze_angle_left += stick_squeeze_speed * Time.fixedDeltaTime; // 마우스 왼쪽 버튼을 누르고 있으면 stick_squeeze_angle이 초당 stick_squeeze_speed만큼 증가합니다. 
            if (stick_squeeze_angle_left > stick_squeeze_max_angle) // stick_squeeze_angle이 최대를 넘어가면
            {
                stick_squeeze_angle_left = stick_squeeze_max_angle; // 최대로 고정합니다.
            }
        }
        else
        {
            if (stick_squeeze_angle_left > stick_squeeze_min_angle) //stick_squeeze_angle이 최소보다 크면
                stick_squeeze_angle_left -= stick_squeeze_speed * 2 * Time.fixedDeltaTime; // 마우스 왼쪽 버튼을 누르고 있지 않으면 stick_squeeze_angle이 초당 2 * stick_squeeze_speed 만큼 감소합니다.
            if (stick_squeeze_angle_left < stick_squeeze_min_angle) // stick_squeeze_angle이 최소보다 작아지면
            {
                stick_squeeze_angle_left = stick_squeeze_min_angle; // 최솟값으로 고정합니다.
            }
        }

        if (Input.GetMouseButton(1))
        {
            stick_squeeze_angle_right += stick_squeeze_speed * Time.fixedDeltaTime; // 마우스 왼쪽 버튼을 누르고 있으면 stick_squeeze_angle이 초당 stick_squeeze_speed만큼 증가합니다. 
            if (stick_squeeze_angle_right > stick_squeeze_max_angle) // stick_squeeze_angle이 최대를 넘어가면
            {
                stick_squeeze_angle_right = stick_squeeze_max_angle; // 최대로 고정합니다.
            }
        }
        else
        {
            if (stick_squeeze_angle_right > stick_squeeze_min_angle) //stick_squeeze_angle이 최소보다 크면
                stick_squeeze_angle_right -= stick_squeeze_speed * 2 * Time.fixedDeltaTime; // 마우스 왼쪽 버튼을 누르고 있지 않으면 stick_squeeze_angle이 초당 stick_squeeze_speed 만큼 감소합니다.
            if (stick_squeeze_angle_right < stick_squeeze_min_angle) // stick_squeeze_angle이 최소보다 작아지면
            {
                stick_squeeze_angle_right = stick_squeeze_min_angle; // 최솟값으로 고정합니다.
            }
        }

        if (Input.GetKey(KeyCode.UpArrow)) // 위쪽 화살표 키를 누르면
        {
            main_axis_height -= main_axis_height_change_speed;
            if (main_axis_height < main_axis_height_min) // main_axis_height가 최소를 넘어가면
            {
                main_axis_height = main_axis_height_min; // 최솟값으로 고정합니다.
            }
        }
        if (Input.GetKey(KeyCode.DownArrow)) // 아래쪽 화살표 키를 누르면
        {
            main_axis_height += main_axis_height_change_speed;
            if (main_axis_height > main_axis_height_max) // main_axis_height가 최대를 넘어가면
            {
                main_axis_height = main_axis_height_max; // 최대로 고정합니다.
            }
        }
        if (Input.mouseScrollDelta.y > 0 && !is_locked) // 오른쪽 화살표 키를 누르면
        {
            main_axis_width += main_axis_width_change_speed;
            if (main_axis_width > main_axis_width_max) // main_axis_width가 최대를 넘어가면
            {
                main_axis_width = main_axis_width_max; // 최대로 고정합니다.
            }
            axis_width_increased = true; // main_axis_width가 증가했음을 표시합니다.
        }
        if (Input.mouseScrollDelta.y < 0 && !is_locked) // 왼쪽 화살표 키를 누르면 (is_locked가 false일 때만 작동합니다. 락걸렸을 때는 )
        {
            main_axis_width -= main_axis_width_change_speed;
                if (main_axis_width < main_axis_width_min) // main_axis_width가 최소를 넘어가면
                {
                    main_axis_width = main_axis_width_min; // 최솟값으로 고정합니다.
                }
            if (is_locked && main_axis_width > main_axis_width_min)
            {
                angle_on_collision_left -= 1.2f; // !!!!!!!!!!!!!!!! 이거 야매로 작동만 하게 때워둔 거라  main_axis_width_change_speed 바뀌면 오류 발생 가능성 있음!@@!@!@!@!!@!@!@!@!@!@!
                angle_on_collision_right -= 1.2f;
            }
            axis_width_decreased = true; // main_axis_width가 감소했음을 표시합니다.
        }

        stick_left_axis.localPosition = new Vector3(-main_axis_width / 2f, 0f, 0f); // 왼쪽 젓가락 축의 위치를 설정합니다. 왼쪽 젓가락은 왼쪽으로 이동해야 하므로 x좌표는 -main_axis_width / 2f로 설정합니다.
        stick_right_axis.localPosition = new Vector3(main_axis_width / 2f, 0f, 0f); // 오른쪽 젓가락 축의 위치를 설정합니다. 오른쪽 젓가락은 오른쪽으로 이동해야 하므로 x좌표는 main_axis_width / 2f로 설정합니다.
        left_target.localPosition = new Vector3(0f, -main_axis_height, 0f); // 왼쪽 젓가락 타겟의 위치를 설정합니다. y좌표는 -main_axis_height로 설정합니다.
        right_target.localPosition = new Vector3(0f, -main_axis_height, 0f); // 오른쪽 젓가락 타겟의 위치를 설정합니다. y좌표는 -main_axis_height로 설정합니다.

        if (!is_locked)
        {
            left_stick.ToTargetPosition(stick_squeeze_angle_left);
            right_stick.ToTargetPosition(-stick_squeeze_angle_right); // 왼쪽, 오른쪽 젓가락을 목표 위치로 이동시키는 함수 호출
            stick_average_velocity = (left_stick_velocity + right_stick_velocity) / 2f; // 왼쪽, 오른쪽 젓가락의 평균 속도를 계산합니다.

            

            if (left_stick.is_touching_stick && right_stick.is_touching_stick) // 왼쪽, 오른쪽 젓가락이 닿아있으면
            {
                is_locked = true; // 젓가락이 서로 맞닿아 더 이상 움직이지 못하는 상태로 설정합니다.
                angle_on_collision_left = stick_squeeze_angle_left; // 현재 stick_squeeze_angle을 angle_on_collision에 저장합니다.
                angle_on_collision_right = stick_squeeze_angle_right; // 현재 stick_squeeze_angle을 angle_on_collision에 저장합니다.
                stick_left_axis.localRotation = Quaternion.Euler(0, 0, angle_on_collision_left); // 왼쪽 젓가락 축의 회전값을 stick_squeeze_angle로 설정합니다.
                stick_right_axis.localRotation = Quaternion.Euler(0, 0, -angle_on_collision_right);
            }
            else
            {
                stick_left_axis.localRotation = Quaternion.Euler(0, 0, stick_squeeze_angle_left); // 왼쪽 젓가락 축의 회전값을 stick_squeeze_angle로 설정합니다.
                stick_right_axis.localRotation = Quaternion.Euler(0, 0, -stick_squeeze_angle_right); // 오른쪽 젓가락 축의 회전값을 -stick_squeeze_angle로 설정합니다. -인 이유는 오른쪽 젓가락이 왼쪽 젓가락과 반대 방향으로 회전해야 오므려지기 때문입니다.

            }
        }
        else
        {
            left_stick_velocity = left_stick.ToTargetPosition(angle_on_collision_left);
            right_stick_velocity = right_stick.ToTargetPosition(-angle_on_collision_right); // 왼쪽, 오른쪽 젓가락을 목표 위치로 이동시키는 함수 호출


            foreach (var target in holdingTargets)
            {
                if (!left_stick.collidingObjects.ContainsKey(target) || !right_stick.collidingObjects.ContainsKey(target))
                {
                    holdingTargets.Remove(target);
                    if (holdingTargets.Count == 0)
                    {
                        is_locked_by_target = false; // holdingTargets가 비어있으면 is_locked_by_target를 false로 설정합니다.
                    }
                }

            }

            if (!left_stick.is_touching_stick && !right_stick.is_touching_stick && !is_locked_by_target) // 왼쪽, 오른쪽 젓가락이 닿아있지 않으면 (뭐 잡고있지 않을 때)
            {
                is_locked = false; // 젓가락이 서로 맞닿아 더 이상 움직이지 못하는 상태에서 벗어납니다.
                stick_squeeze_angle_left = angle_on_collision_left; // 젓가락의 앵글을 angle_on_collision으로 되돌립니다.
                stick_squeeze_angle_right = angle_on_collision_right; // 젓가락의 앵글을 angle_on_collision으로 되돌립니다.
            }


            if (angle_on_collision_left != stick_squeeze_min_angle && angle_on_collision_right != stick_squeeze_min_angle)
            {
                if (!Input.GetMouseButton(0) || !Input.GetMouseButton(1)) // 마우스 왼쪽 버튼이나 오른쪽 버튼을 떼면
                {
                    is_locked = false;
                    stick_squeeze_angle_left = angle_on_collision_left - 1; // 젓가락의 앵글을 angle_on_collision으로 되돌립니다. -1을 해주는 이유는 angle_on_collision 값 자체가 충돌할 때 값이라 이걸 그대로 쓰면 다음 프레임에 위의 락 발동이 다시 걸려버리기 때문입니다.
                    stick_squeeze_angle_right = angle_on_collision_right - 1; // 젓가락의 앵글을 angle_on_collision으로 되돌립니다.
                }
                if (stick_squeeze_angle_left < angle_on_collision_left || stick_squeeze_angle_right < angle_on_collision_right)
                {
                     is_locked = false; // 만약 젓가락이 물체와 충돌하지 않으면 is_locked를 false로 설정합니다.
                }
            }
            else if (angle_on_collision_left == stick_squeeze_min_angle)
            {
                if (!Input.GetMouseButton(1)) // 마우스 왼쪽 버튼을 떼면
                {
                    is_locked = false;
                    stick_squeeze_angle_left = angle_on_collision_left - 1; // 젓가락의 앵글을 angle_on_collision으로 되돌립니다.
                    stick_squeeze_angle_right = angle_on_collision_right - 1;
                }
            }
            else if (angle_on_collision_right == stick_squeeze_min_angle)
            {
                if (!Input.GetMouseButton(0)) // 마우스 오른쪽 버튼을 떼면
                {
                    is_locked = false;
                    stick_squeeze_angle_right = angle_on_collision_right - 1; // 젓가락의 앵글을 angle_on_collision으로 되돌립니다.
                    stick_squeeze_angle_left = angle_on_collision_left - 1;
                }
            } 
            
            

            if (!is_locked)
            {
                is_locked_by_target = false; // is_locked가 false로 설정되면 is_locked_by_target도 false로 설정합니다.
            }
        }

        axis_width_increased = false; // 매 프레임마다 axis_width_increased를 false로 초기화합니다. 이는 왼/오 젓가락 스프라이트의 너비가 증가했는지 여부를 나타내는 변수입니다
        axis_width_decreased = false; // 매 프레임마다 axis_width_decreased를 false로 초기화합니다. 이는 왼/오 젓가락 스프라이트의 너비가 감소했는지 여부를 나타내는 변수입니다.
    }
}
