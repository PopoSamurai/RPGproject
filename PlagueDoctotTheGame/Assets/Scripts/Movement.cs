using UnityEngine;
public class Movement : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5f;
    public bool move;
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
    GameObject gameM;
    public bool attack = false;
    public Vector3 direction;
    public Animator anim;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameM = GameObject.FindGameObjectWithTag("GameManager");
    }
    void Update()
    {
        if (move == true)
        {
            direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            direction = direction.normalized;
            //walk anim
            if(direction != Vector3.zero)
            {
                anim.SetBool("move", true);
            }
            else
                anim.SetBool("move", false);

            transform.Translate(direction * speed * Time.deltaTime);

            if (Input.GetMouseButtonDown(0) && attack == false)
            {
                attack = true;
                move = false;
                Invoke("AttackCo", 0.3f);
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.F) && interact.activeSelf == true)
        {
            interactOn = true;
            interact.SetActive(false);
            move = false;
        }
        else
        {
            interactOn = false;
        }

        if (collectInteract.activeSelf == true || dialogOn == true || attack == true)
        {
            move = false;
        }
        else
        {
            move = true;
        }
    }
    void AttackCo()
    {
        //
        attack = false;
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
}