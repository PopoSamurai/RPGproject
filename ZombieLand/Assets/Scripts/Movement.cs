using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    public float speed = 200f, rotationS = 200f;
    float horizontalV, verticalV;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        horizontalV = Input.GetAxisRaw("Horizontal");
        verticalV = Input.GetAxisRaw("Vertical");
        if(verticalV != 0)
            anim.SetBool("move", true);
        else
            anim.SetBool("move", false);
        Move();
    }
    public void Move()
    {
        rb.velocity = (transform.forward * verticalV) * speed * Time.deltaTime;
        transform.Rotate((transform.up * horizontalV) * rotationS * Time.deltaTime);
    }
}
