using UnityEngine;

public class movement : MonoBehaviour
{
    public int speed = 5;
    private Rigidbody2D rb;
    private Animator anim;
    private Camera cam;

    private float moveX;
    private float moveY;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = (mousePos - transform.position).normalized;

        anim.SetFloat("floatX", lookDir.x);
        anim.SetFloat("floatY", lookDir.y);
        anim.SetBool("moveChar", moveX != 0 || moveY != 0);
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveX * speed, moveY * speed);
    }
}