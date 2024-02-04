using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    GameObject player;
    public float smoothing;
    public Vector2 maxPos;
    public Vector2 minPos;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        //transform.position = player.transform.position;
        if(transform.position != player.transform.position)
        {
            Vector3 targetPos = new Vector3(player.transform.position.x,
                                            player.transform.position.y,
                                            -10);
            targetPos.x = Mathf.Clamp(targetPos.x,minPos.x, maxPos.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);
            transform.position = Vector3.Lerp(transform.position,
                                                targetPos, smoothing);
        }
    }
}
