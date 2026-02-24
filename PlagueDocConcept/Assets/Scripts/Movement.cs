using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Rigidbody rb;
    private Camera mainCam;
    private Vector3 moveInput;
    private bool isGrounded;
    private float groundCheckDistance = 0.3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        mainCam = Camera.main;
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        if (mainCam != null)
        {
            Vector3 camF = mainCam.transform.forward;
            Vector3 camR = mainCam.transform.right;

            camF.y = 0;
            camR.y = 0;
            camF.Normalize();
            camR.Normalize();
            moveInput = camF * input.z + camR * input.x;
        }
        else
        {
            moveInput = input;
        }
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance);
    }
    void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;
        Vector3 horizontalMove = moveInput * moveSpeed;
        velocity.x = horizontalMove.x;
        velocity.z = horizontalMove.z;

        rb.velocity = velocity;
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}