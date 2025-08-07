using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class ShadowController : MonoBehaviour
{
    Transform shadow;

    SpriteRenderer shadowRenderer;

    RotationConstraint shadowRC;

    Collider2D col;

    public float tableY = 0f;

    public float maxAlpha = 1f;

    public float minY = 0f;
    void Start()
    {
        col = GetComponent<Collider2D>();
        minY = col.bounds.size.y / 2f;
        tableY = -2.17f;

        shadow = transform.GetChild(0);
        shadowRenderer = shadow.GetComponent<SpriteRenderer>();
        shadowRC = shadow.GetComponent<RotationConstraint>();
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = this.transform;
        source.weight = 1f;
        shadowRC.AddSource(source);
        shadowRC.constraintActive = true;
        shadowRC.locked = true;
        shadowRC.weight = 1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 shadowPos = shadow.position;
        //shadowPos.x = transform.position.x;//0f;
        //shadowPos.y = tableY;//-transform.position.y + tableY;
        shadow.position = shadowPos;

        float t = Mathf.InverseLerp(minY, minY + 3f, transform.position.y);
        float alpha = Mathf.Lerp(maxAlpha, 0f, t);
        Color color = shadowRenderer.color;
        color.a = alpha;
        shadowRenderer.color = color;
    }
    //void LateUpdate() {
    //Quaternion parentRotation = transform.rotation;
    //shadow.localRotation = Quaternion.Inverse(parentRotation);
    //}
    void LateUpdate()
    {
        Vector3 shadowPos = shadow.position;
        shadowPos.x = transform.position.x;
        shadowPos.y = tableY;
        shadow.position = shadowPos;

    }
}
