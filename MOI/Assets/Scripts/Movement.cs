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
    void Start()
    {
        BashTimeReset = BashTime;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        direction = Input.GetAxis("Horizontal") * speed;
        if(direction > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        Jump();
        Bash();
    }
    private void FixedUpdate()
    {
        if(IsBashing == false)
        rb.velocity = new Vector2(direction * Time.fixedDeltaTime, rb.velocity.y);
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
            else if(IsChosingDir && Input.GetKeyUp(KeyCode.Mouse1))
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
