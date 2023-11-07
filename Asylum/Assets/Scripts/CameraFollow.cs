using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1, 10)]
    public float smooth;
    public Vector3 minVal, maxVal;
    private void Update()
    {
        Follow();
    }
        void Follow()
    {
        Vector3 targetPos = target.position + offset;
        Vector3 boundPos = new Vector3(
            Mathf.Clamp(targetPos.x, minVal.x, maxVal.x),
            Mathf.Clamp(targetPos.y, minVal.y, maxVal.y),
            Mathf.Clamp(targetPos.z, minVal.z, maxVal.z)
            );
        Vector3 smoothPos = Vector3.Lerp(transform.position, boundPos, smooth * Time.fixedDeltaTime);
        transform.position = smoothPos;
    }
}
