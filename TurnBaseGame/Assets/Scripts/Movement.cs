using BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace interactOn
{
    public class Movement : MonoBehaviour
    {
        Rigidbody2D rb;
        Animator anim;
        [SerializeField] Transform groundCheck;
        [SerializeField] LayerMask groundLayer;

        const float groundCheckRadius = 0.2f;
        public float speed = 3;
        float horizontalVal;
        bool facingRight = true;
        [SerializeField] bool isGround;
        public AudioSource step1;
        public AudioSource step2;
        public bool move = true;
        public void MoveOn()
        {
            move = true;
        }
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }
        private void Update()
        {
            if (move == true)
            {
                horizontalVal = Input.GetAxisRaw("Horizontal");
                Move(horizontalVal);
                GroundCheck();
            }
            else
            {
                Move(0);
            }
        }
        void GroundCheck()
        {
            isGround = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
            if (colliders.Length > 0)
                isGround = true;
        }
        public void Move(float dir)
        {
            float xVal = dir * speed * 100 * Time.fixedDeltaTime;
            Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
            rb.velocity = targetVelocity;

            Vector3 currentScale = transform.localScale;
            //odwrót
            if (facingRight && dir < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                facingRight = false;
            }
            else if (!facingRight && dir > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                facingRight = true;
            }
            //idle = 0, walk = 6
            anim.SetFloat("vVelocity", Mathf.Abs(rb.velocity.x));
        }
        //kroki
        public void Step1()
        {
            step1.Play();
        }
        public void Step2()
        {
            step2.Play();
        }
    }
}