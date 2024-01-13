using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 pos;

    public GameObject robot;
    public Camera cam;
    private Vector3 roboPos;
    private RectTransform rt;
    private RectTransform canvasRT;
    private Vector3 roboScreenPos;

    void Start()
    {
        roboPos = robot.transform.position;

        rt = GetComponent<RectTransform>();
        canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        roboScreenPos = cam.WorldToViewportPoint(robot.transform.TransformPoint(roboPos));
        rt.anchoredPosition = roboScreenPos + new Vector3(200, -265, 0);
    }

    void Update()
    {
        rt.anchoredPosition = roboScreenPos + new Vector3(200, -265, 0);

        roboScreenPos = cam.WorldToViewportPoint(robot.transform.TransformPoint(roboPos));
        rt.anchorMax = roboScreenPos;
        rt.anchorMin = roboScreenPos;
    }
}