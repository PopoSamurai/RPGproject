using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public Transform cubeBottom;
    public Transform cubeTop;
    LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        lr.SetPosition(0, cubeBottom.transform.localPosition);
        lr.SetPosition(1, cubeTop.transform.localPosition);
    }
}
