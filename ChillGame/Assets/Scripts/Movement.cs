using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    public float speed;
    public Vector3 change;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        updateAnimationAndMove();
    }
    void updateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            anim.SetFloat("moveX", change.x);
            anim.SetFloat("moveY", change.y);
            anim.SetBool("move", true);
        }
        else
        {
            anim.SetBool("move", false);
        }
    }
    public void MoveCharacter()
    {
        rb.MovePosition(transform.position + change.normalized * speed * Time.fixedDeltaTime);
    }
}
