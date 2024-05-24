using UnityEngine;
public class Movement : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5f;
    public static bool move = true;
    public Vector3 direction;
    public bool dialogOn = false;
    public bool interactOn = false;
    public GameObject interact;
    public Animator anim;
    bool facingRight = true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (move == true)
        {
            direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            direction = direction.normalized;
            transform.Translate(direction * speed * Time.deltaTime);
            //flip
            if (facingRight && direction.x < 0)
            {
                anim.gameObject.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
                facingRight = false;
            }
            else if (!facingRight && direction.x > 0)
            {
                anim.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                facingRight = true;
            }
            if (direction != Vector3.zero)
                anim.SetBool("move", true);
            else anim.SetBool("move", false);

            if (Input.GetKeyDown(KeyCode.E))
            {
                interactOn = true;
                Invoke("InteractOff", 0.2f);
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    public void InteractOff()
    {
        interactOn = false;
    }
    public void MoveOn()
    {
        move = true;
    }
    public void MoveOff()
    {
        move = false;
    }
}