using UnityEngine;

public class ShadowItself : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate() {
    Quaternion parentRotation = transform.parent.rotation;
    transform.localRotation = Quaternion.Inverse(parentRotation);
}
}
