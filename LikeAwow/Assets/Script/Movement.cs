using System;
using UnityEngine;
public class Movement : MonoBehaviour
{
    public GameObject[] poses; // 1 - down / 2 - up / 3 - left / 4 - right
    public int activeIndex = 0;
    int index = 0;
    private int previousIndex;
    Rigidbody rb;
    public float speed;
    Vector3 change;

    public Animator[] animators;
    public bool walk = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        walk = false;
        previousIndex = index;
    }
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.z = Input.GetAxisRaw("Vertical");

        for (int i = 0; i < poses.Length; i++)
        {
            if (i == index)
            {
                poses[i].SetActive(true);
                if (walk == true)
                {
                    animators[i].SetBool("walk", true);
                }
                else animators[i].SetBool("walk", false);
            }
            else
            {
                poses[i].SetActive(false);
            }
        }

        if (change != Vector3.zero)
        {
            MoveCharacter();
            walk = true;
        }
        else if (change != Vector3.zero && index != previousIndex)
        { 
            MoveCharacter();
            walk = true;
        }
        else
        {
            walk = false;
        }

        if (Input.GetKey(KeyCode.S))
            index = 0;
        if (Input.GetKey(KeyCode.W))
            index = 1;
        if (Input.GetKey(KeyCode.A))
            index = 2;
        if (Input.GetKey(KeyCode.D))
            index = 3;
    }
    public void MoveCharacter()
    {
        rb.MovePosition(transform.position + change.normalized * speed * Time.fixedDeltaTime);
    }
}