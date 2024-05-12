using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roateModel : MonoBehaviour
{
    public float speedMouse;
    Ray cameraRay;
    RaycastHit cameraRayHit;

    void Update()
    {
        //aim
        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraRay, out cameraRayHit, 1000f, 3))
        {
            if (cameraRayHit.transform.tag == "Ground")
            {
                Vector3 targetPosition = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
                transform.LookAt(targetPosition);
            }
        }

    }
}
