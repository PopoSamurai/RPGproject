using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
public class Movement : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5f;
    public float speedMouse;
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
    [SerializeField] GameObject attackPrefab;
    public bool attack = false;
    public Vector3 direction;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameM = GameObject.FindGameObjectWithTag("GameManager");
    }
    void Update()
    {
        if (move == true)
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
        else
        {
            interactOn = false;
        }

        if (Input.GetMouseButtonDown(0) && attack == false)
        {
            attack = true;
            attackPrefab.SetActive(true);
            move = false;
            StartCoroutine(AttackCo());
        }

        if (collectInteract.activeSelf == true || dialogOn == true || gameM.GetComponent<GameM>().CraftPanelWin.activeSelf == true || attack == true)
        {
            move = false;
        }
        else
        {
            move = true;
        }
    }
    IEnumerator AttackCo()
    {
        yield return new WaitForSeconds(0.3f);
        attackPrefab.SetActive(false);
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
    public void Move()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }
}