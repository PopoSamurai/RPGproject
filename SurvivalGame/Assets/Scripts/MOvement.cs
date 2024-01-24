using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class MOvement : MonoBehaviour
{
    GameObject Aim;
    public float speed;
    private float horizontalInput, verticalInput;
    private Vector3 playerVelocity;
    void Start()
    {
        Aim = GameObject.FindGameObjectWithTag("Aim");
    }

    void Update()
    {
        //roate
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.cyan);

            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
        //move
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        playerVelocity = new Vector3(horizontalInput, 0, verticalInput);
        transform.position += playerVelocity * speed * Time.deltaTime;
    }
}
