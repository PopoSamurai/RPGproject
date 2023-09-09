using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    float horizontalVal;
    Animator anim;
    Rigidbody2D rb;
    public bool isGround, isRunning, isAttack;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    const float groundCheckRadius = 0.2f;
    public float runSpeed = 2f;
    public float jumpPower = 8;
    [SerializeField] int totalJumps;
    int availableJumps;
    bool facingRight = true;
    bool multipleJumps;
    bool coyoteJump;
    public Transform weamponPoint;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        horizontalVal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isGround)
            {
                isRunning = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;

        if (Input.GetButtonDown("Jump"))
            Jump();
        if (Input.GetMouseButtonDown(0))
        {
            isAttack = true;
            anim.SetBool("attack", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isAttack = false;
            anim.SetBool("attack", false);
        }

    }
    private void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalVal);
    }
    public void GroundCheck()
    {
        bool wasGrounded = isGround;
        isGround = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
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
            if (wasGrounded)
                StartCoroutine(coyoteJumpDelay());
        }
        anim.SetBool("jump", !isGround);
    }

    public void Move(float dir)
    {
        #region Move
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        if (isRunning)
            xVal *= runSpeed;
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        Vector3 currentScale = transform.localScale;
        //odwr t
        if (facingRight && dir < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        else if (!facingRight && dir > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }
        //idle = 0, walk = 4, run = 8
        anim.SetFloat("vVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    }
    IEnumerator coyoteJumpDelay()
    {
        coyoteJump = true;
        yield return new WaitForSeconds(0.3f);
        coyoteJump = false;
    }
    public void Jump()
    {
        if (isGround)
        {
            multipleJumps = true;
            availableJumps--;

            rb.velocity = Vector2.up * jumpPower;
            anim.SetBool("jump", true);
        }
        else
        {
            if (coyoteJump)
            {
                multipleJumps = true;
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                anim.SetBool("jump", true);
            }
            if (multipleJumps && availableJumps > 0)
            {
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                anim.SetBool("jump", true);
            }
        }
    }
}
