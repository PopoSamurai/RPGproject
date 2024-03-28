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
    public bool dialogOn = false;
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

        if (Input.GetKeyDown(KeyCode.F) && interact.activeSelf)
        {
            interactOn = true;
            interact.SetActive(false);
            move = false;
        }
        else interactOn = false;

        if(collectInteract.activeSelf == true || dialogOn == true)
            move = false;
        else
            move = true;
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
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "NPC")
        {
            interact.SetActive(true);
        }
        if (other.tag == "interactive")
        {
            interact.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        interact.SetActive(false);
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