using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    [HideInInspector]
    public Vector2 direction;
    SpriteRenderer sr;
    //fix flip
    float lastHorizontalVec;
    float lastVerticalVec;
    [HideInInspector]
    public Vector2 lastmovement;

    //References
    public CharacterScriptableObj characterData;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastmovement = new Vector2(1, 0f);
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        InputManager();

        //anims
        if(direction.x != 0 || direction.y != 0)
        {
            anim.SetBool("move", true);
            ChangePos();
        }
        else anim.SetBool("move", false);
    }
    public void ChangePos()
    {
        if(lastHorizontalVec < 0) sr.flipX = false;
        else sr.flipX = true;
    }
    public void FixedUpdate()
    {
        Move();
    }
    public void InputManager()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        direction = new Vector2(moveX, moveY).normalized;

        if (direction.x != 0)
        {
            lastHorizontalVec = direction.x;
            lastmovement = new Vector2(lastHorizontalVec, 0f);
        }
        if (direction.y != 0)
        {
            lastVerticalVec = direction.y;
            lastmovement = new Vector2(0f, lastVerticalVec);
        }
        if(direction.x != 0 && direction.y != 0)
        {
            lastmovement = new Vector2(lastHorizontalVec, lastVerticalVec);
        }
    }
    public void Move()
    {
        rb.velocity = new Vector2(direction.x * characterData.MoveSpeed, direction.y * characterData.MoveSpeed);
    }
}