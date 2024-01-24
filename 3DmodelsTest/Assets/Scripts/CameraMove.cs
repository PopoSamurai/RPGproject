using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        player = GameObject.Find("Player");
    }
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + offset, ref velocity, smoothSpeed * Time.deltaTime);
    }
}
