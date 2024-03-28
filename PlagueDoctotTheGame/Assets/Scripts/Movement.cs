using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5f;
    public float speedMouse;
    public bool move = true;
    public int posPlayer = 2;
    [Header("Interact")]
    public GameObject interact;
    public GameObject collectInteract;
    public GameObject collectTxt;
    public GameObject LoadBar;
    public bool interactClick;
    //
    public bool interactOn = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (move)
        {
            Move();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            interactOn = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "NPC" || collision.transform.tag == "Enemy")
            collision.rigidbody.isKinematic = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "NPC" || collision.transform.tag == "Enemy")
            collision.rigidbody.isKinematic = false;
    }
    public void MoveOn()
    {
        move = true;
    }
    public void MoveOff()
    {
        move = false;
    }
    public void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.velocity = move.normalized * speed;
    }
}