using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    public float direction;
    //jump
    public float jumpPower;
    public LayerMask layerMask;
    Collider2D coll;
    public GameObject addToon;

    [Header("Bash")]
    public float radius;
    public GameObject BashAbleObj;
    bool NearToBashAbleObj;
    bool IsChosingDir;
    bool IsBashing;
    public float BashPower;
    public float BashTime;
    public GameObject Arrow;
    Vector3 BashDir;
    float BashTimeReset;
    //dash
    bool canDash = true;
    bool isDashing = false;
    public float dashingPower = 24f;
    float dashingTime = 0.2f;
    float dashingCooldown = 0.5f;
    [SerializeField] private TrailRenderer tr;
    //hold
    public float holdJump;
    public float holder = 0f;
    float maxJump = 1.5f;
    public GameObject loadEffect;
    public bool isMove = true;
    void Start()
    {
        BashTimeReset = BashTime;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isMove == true)
        {
            if (isDashing)
            {
                return;
            }
            if (IsBashing == false)
                rb.velocity = new Vector2(direction * Time.fixedDeltaTime, rb.velocity.y);

            direction = Input.GetAxis("Horizontal") * speed;
            if (direction > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
            Bash();
            Jump();
        }
        if (Input.GetButton("KeyS") && !isGrounded())
        {
            rb.velocity = Vector2.zero;
            rb.velocity = Vector2.down * 40f;
        }
        if (Input.GetButton("KeyW") && isGrounded())
        {
            rb.velocity = Vector2.zero;
            isMove = false;
            addToon.SetActive(false);
            loadEffect.SetActive(true);
            if(holder < maxJump)
            holder += Time.deltaTime;
            holdJump = holder;
        }
        else
        {
            isMove = true;
            addToon.SetActive(true);
            loadEffect.SetActive(false);
            rb.AddForce(transform.up * holdJump * 250);
            StartCoroutine(waitHolder());
        }
    }
    IEnumerator waitHolder()
    {
        yield return new WaitForSeconds(0.2f);
        holdJump = 0f;
        holder = 0f;
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if(direction > 1)
            rb.velocity = Vector2.right * dashingPower;
        else
            rb.velocity = Vector2.left * dashingPower;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rb.AddForce(transform.up * jumpPower);
        }
    }
    bool isGrounded()
    {
        float ExtraHight = 0.03f;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, ExtraHight, layerMask);
        return raycastHit2D.collider != null;
    }
    //Arrow
    void Bash()
    {
        RaycastHit2D[] Rays = Physics2D.CircleCastAll(transform.position, radius, Vector3.forward);
        foreach(RaycastHit2D ray in Rays)
        {
            NearToBashAbleObj = false;

            if(ray.collider.tag == "Bashable")
            {
                NearToBashAbleObj = true;
                BashAbleObj = ray.collider.transform.gameObject;
                break;
            }
        }
        if(NearToBashAbleObj)
        {
            BashAbleObj.GetComponent<SpriteRenderer>().color = Color.yellow;
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                addToon.SetActive(false);
                Time.timeScale = 0;
                BashAbleObj.transform.localScale = new Vector2(1.4f, 1.4f);
                Arrow.SetActive(true);
                Arrow.transform.position = BashAbleObj.transform.transform.position;
                IsChosingDir = true;
            }
            else if(IsChosingDir && Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                addToon.SetActive(true);
                Time.timeScale = 1f;
                BashAbleObj.transform.localScale = new Vector2(1, 1);
                IsChosingDir = false;
                IsBashing = true;
                rb.velocity = Vector2.zero;
                transform.position = BashAbleObj.transform.position;
                BashDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                BashDir.z = 0;
                if(BashDir.x > 0)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                BashDir = BashDir.normalized;
                BashAbleObj.GetComponent<Rigidbody2D>().AddForce(-BashDir * 50, ForceMode2D.Impulse);
                Arrow.SetActive(false);
            }
        }
        else if(BashAbleObj != null)
        {
            BashAbleObj.GetComponent<SpriteRenderer>().color = Color.white;
        }
        //
        if(IsBashing)
        {
            if(BashTime > 0)
            {
                BashTime -= Time.deltaTime;
                rb.velocity = BashDir * BashPower * Time.deltaTime;
            }
            else
            {
                IsBashing = false;
                BashTime = BashTimeReset;
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);   
    }
}
