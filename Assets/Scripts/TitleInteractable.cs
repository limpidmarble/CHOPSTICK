using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleInteractable : MonoBehaviour
{
//    public Sprite fullSceneSprite;
//    public Sprite holeSceneSprite;

    public GameObject gameStart;

    Rigidbody2D startRigidbody;

    Collider2D startCollider;

    public Vector2 startVelocity;

    private SpriteRenderer spriteRenderer;

    Vector3 originalStartPosition = new Vector3(3.851f, -0.742f, 0f);

    public bool isStarting = false;

    public bool canStart = false;
    bool reachedOriginalPosition = false;

    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startRigidbody = gameStart.GetComponent<Rigidbody2D>();
        startCollider = gameStart.GetComponent<Collider2D>();

        startRigidbody.linearVelocity = startVelocity;

        StartCoroutine(WaitAndEnable(2f));
    }

    // Update is called once per frame


    public void InitiateStart()
    {
        startRigidbody.gravityScale = 0f;
        startCollider.isTrigger = true;
        isStarting = true;
    }

    void FixedUpdate()
    {
        if (isStarting)
        {
            Vector2 newPosition = Vector2.MoveTowards(startRigidbody.position, originalStartPosition, moveSpeed * Time.fixedDeltaTime);
            startRigidbody.MovePosition(newPosition);

            // 회전 보간 (MoveRotation)
            float angle = Mathf.MoveTowardsAngle(startRigidbody.rotation, 0, rotateSpeed * Time.fixedDeltaTime);
            startRigidbody.MoveRotation(angle);
            if (Vector2.Distance(startRigidbody.position, originalStartPosition) < 0.01f && Mathf.Abs(Mathf.DeltaAngle(startRigidbody.rotation, 0)) < 0.5f)
            {
            reachedOriginalPosition = true;
            SceneManager.LoadScene("MainGame");
            }
        }

    }

    IEnumerator WaitAndEnable(float waitTime)
    {
        Debug.Log("대기 시작");
        // waitTime 초 동안 대기
        yield return new WaitForSeconds(waitTime);
        canStart = true;
    }
        
}
