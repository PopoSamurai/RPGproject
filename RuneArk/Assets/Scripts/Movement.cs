using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Rune
{
    public class Movement : MonoBehaviour
    {
        Rigidbody2D rb;
        Animator anim;
        [SerializeField] Collider2D standCol;
        [SerializeField] Transform groundCheck;
        [SerializeField] Transform headCheck;
        [SerializeField] Transform wallCheckCollider;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] LayerMask wallLayer;
        const float groundCheckRadius = 0.2f;
        const float overHeadRadius = 0.2f;
        const float wallCheckRadius = 0.2f;
        public float speed = 3;
        float horizontalVal;
        public float runSpeed = 2f;
        public float crounchSpeed = 0.5f;
        public float slideFactor = 0.2f;
        public float jumpPower = 8;
        [SerializeField] int totalJumps;
        public float wallSlidingSpeed;
        int availableJumps;
        bool facingRight = true;
        bool isGround;
        bool isRunning = false;
        bool isCrounching = false;
        bool coyoteJump;
        bool multipleJumps;
        bool isPushing = false;
        bool isPushingLeft = false;
        public bool isSliding;
        //push
        public float distance = 1f;
        public LayerMask boxMask;
        GameObject box;
        private void Awake()
        {
            availableJumps = totalJumps;
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (CanMove() == false)
                return;
            anim.SetFloat("yVelocity", rb.velocity.y);
            horizontalVal = Input.GetAxisRaw("Horizontal");

            if(Input.GetKeyDown(KeyCode.LeftShift) && !isCrounching && isPushing == false)
            {
                if (isGround)
                {
                    isRunning = true;
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && !isCrounching)
                isRunning = false;

            if (Input.GetButtonDown("Jump") && isPushing == false)
                Jump();

            if (Input.GetButton("Crounch") && isPushing == false)
            {
                if (isGround)
                {
                    isCrounching = true;
                }
            }
            else
            {
                if (isGround)
                {
                    isCrounching = false;
                }
            }
            WallCheck();
            //push
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, boxMask);

            if (hit.collider != null && hit.collider.gameObject.tag == "pushable" && Input.GetKeyDown(KeyCode.E))
            { 
                if (facingRight == true)
                {
                    isPushing = true;
                }
                else
                {
                    isPushingLeft = true;
                }
                anim.SetBool("push", true);
                box = hit.collider.gameObject;

                box.GetComponent<FixedJoint2D>().enabled = true;
                box.GetComponent<boxPull>().beingPushed = true;
                box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                anim.SetBool("push", false);
                isPushing = false;
                isPushingLeft = false;
                box.GetComponent<FixedJoint2D>().enabled = false;
                box.GetComponent<boxPull>().beingPushed = false;
            }
        }
        private void FixedUpdate()
        {
            GroundCheck();
            Move(horizontalVal, isCrounching);
        }
        public void WallCheck()
        {
            if(Physics2D.OverlapCircle(wallCheckCollider.position, wallCheckRadius, wallLayer) && Mathf.Abs(horizontalVal) > 0 && rb.velocity.y < 1 && !isGround)
            {
                if (!isSliding)
                {
                    availableJumps = totalJumps;
                    multipleJumps = false;
                }
                Debug.Log("slide");
                anim.SetBool("cathWall", true);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
                isSliding = true;

                if (Input.GetButtonDown("Jump"))
                {
                    availableJumps--;
                    rb.velocity = Vector2.up * jumpPower;
                    anim.SetBool("jump", true);

                    anim.SetBool("cathWall", false);
                    isSliding = false;
                }
            }
            else
            {
                anim.SetBool("cathWall", false);
                isSliding = false;
            }
        }
        bool CanMove()
        {
            bool can = true;
            if (FindObjectOfType<InteractionSystem>().isEximing)
                can = false;
            return can;
        }
        public void Jump()
        {
            if (isGround && !isCrounching)
            {
                multipleJumps = true;
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                anim.SetBool("jump", true);
            }
            else
            {
                if(coyoteJump && !isCrounching)
                {
                    multipleJumps = true;
                    availableJumps--;
                     
                    rb.velocity = Vector2.up * jumpPower;
                    anim.SetBool("jump", true);
                }

                if(multipleJumps && availableJumps > 0)
                {
                    availableJumps--;

                    rb.velocity = Vector2.up * jumpPower;
                    anim.SetBool("jump", true);
                }
            }
        }
        void GroundCheck()
        {
            bool wasGrounded = isGround;
            isGround = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
            if(colliders.Length > 0)
            {
                isGround = true;
                if (!wasGrounded)
                {
                    availableJumps = totalJumps;
                    multipleJumps = false;
                }
            }
            else
            {
                if(wasGrounded)
                    StartCoroutine(coyoteJumpDelay());
            }

            anim.SetBool("jump", !isGround);
        }
        IEnumerator coyoteJumpDelay()
        {
            coyoteJump = true;
            yield return new WaitForSeconds(0.3f);
            coyoteJump = false;
        }
        public void Move(float dir, bool crounchFlag)
        {
            if(isGround)
            {
                if (!crounchFlag)
                {
                    if (Physics2D.OverlapCircle(headCheck.position, overHeadRadius, wallLayer))
                        crounchFlag = true;
                }
                anim.SetBool("crounch", crounchFlag);
                isCrounching = crounchFlag;
                standCol.enabled = !crounchFlag;
            }
            else
            {
                anim.SetBool("crounch", false);
            }
            #region Move
            float xVal = dir * speed * 100 * Time.fixedDeltaTime;
            if (isRunning)
                xVal *= runSpeed;
            if (isCrounching)
                xVal *= crounchSpeed;
            Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
            rb.velocity = targetVelocity;

            Vector3 currentScale = transform.localScale;
            //odwrót
            if(facingRight && dir < 0 && isPushing == false)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                facingRight = false;
            }
            else if (!facingRight && dir > 0 && isPushingLeft == false)
            {
                transform.localScale = new Vector3(1, 1, 1);
                facingRight = true;
            }
            //idle = 0, walk = 4, run = 8
            anim.SetFloat("vVelocity", Mathf.Abs(rb.velocity.x));
            #endregion
        }

        //Gizmosy
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(headCheck.position, overHeadRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * transform.localScale.x * distance);
        }
    }
}