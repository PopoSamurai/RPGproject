using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOvement : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    [SerializeField] Animator rightArm;
    [SerializeField] Animator leftArm;
    public float horizontalVal;
    public int speed;
    public bool move = true;
    bool facingRight = true;
    //groundCheck
    [SerializeField] bool isGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    const float groundCheckRadius = 0.2f;
    //jump
    public float jumpPower = 8;
    [SerializeField] int totalJumps;
    int availableJumps;
    bool multipleJumps;
    bool coyoteJump;
    //Cursor
    private GameObject cursor;

    private GameObject currentWayPlatform;
    [SerializeField] private CapsuleCollider2D playerColl;
    void Start()
    {
        playerColl = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cursor = GameObject.FindGameObjectWithTag("Aim");
    }
    public void MoveOn()
    {
        move = true;
    }
    public void MoveOff()
    {
        move = false;
    }

    void Update()
    {
        horizontalVal = Input.GetAxisRaw("Horizontal");
        if (move == true)
        {
            Move(horizontalVal);
            GroundCheck();
        }
        else
            Move(0);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentWayPlatform != null)
            {
                StartCoroutine(DisableColl());
            }
        }
        //odwrot
        if (cursor.transform.position.x < this.transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        else if(cursor.transform.position.x > this.transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }
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
    public void Move(float dir)
    {
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        Vector3 currentScale = transform.localScale;

        if(dir > 0 || dir < 0)
        {
            if (groundCheck)
            {
                anim.SetBool("walk", true);
            }
            rightArm.GetComponent<Animator>().SetBool("walk", true);
            leftArm.GetComponent<Animator>().SetBool("walk", true);
        }
        else
        {
            if (groundCheck)
            {
                anim.SetBool("walk", false);
            }
            rightArm.GetComponent<Animator>().SetBool("walk", false);
            leftArm.GetComponent<Animator>().SetBool("walk", false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentWayPlatform = collision.gameObject;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentWayPlatform = null;
        }
    }
    void GroundCheck()
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
    IEnumerator coyoteJumpDelay()
    {
        coyoteJump = true;
        yield return new WaitForSeconds(0.3f);
        coyoteJump = false;
    }
    public IEnumerator DisableColl()
    {
        CompositeCollider2D platformCollider = currentWayPlatform.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(playerColl, platformCollider);
        Physics.IgnoreLayerCollision(3, 6, true);
        yield return new WaitForSeconds(1f);
        Physics2D.IgnoreCollision(playerColl, platformCollider, false);
        Physics.IgnoreLayerCollision(3, 6, false);
    }
}
