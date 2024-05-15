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

            if(Input.GetKeyDown(KeyCode.E))
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