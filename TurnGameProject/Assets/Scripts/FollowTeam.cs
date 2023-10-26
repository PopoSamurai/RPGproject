using interactOn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTeam : MonoBehaviour
{
    public Animator anim;
    public float speed = 3f;
    private Transform target;
    public float stopDistance;
    bool facingRight = true;
    public AudioSource step1;
    public AudioSource step2;
    public void Step1()
    {
        step1.Play();
    }
    public void Step2()
    {
        step2.Play();
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    private void Update()
    {
        //anim
        if (target.GetComponent<Movement>().horizontalVal != 0)
        {
            if (Vector2.Distance(transform.position, target.position) > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
                anim.SetBool("move", true);
            }
        }
        else
        {
            anim.SetBool("move", false);
        }
        //odwrót
        if (facingRight && target.GetComponent<Movement>().horizontalVal < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = false;
        }
        else if (!facingRight && target.GetComponent<Movement>().horizontalVal > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = true;
        }
    }
}