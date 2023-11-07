using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorPos : MonoBehaviour
{
    public Vector3 worldPosition;
    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = worldPosition;
    }
}