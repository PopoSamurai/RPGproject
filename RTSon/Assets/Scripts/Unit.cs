using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
public enum Interact
{
    empty,
    work
}
public class Unit : MonoBehaviour
{
    public Interact interact;
    Animator anim;
    NavMeshAgent agent;
    public LayerMask ground, build;
    Camera cam;
    public bool selectOn = false;
    //fixUiclick
    public Sprite icon;
    public string nameV;
    public Sprite skill1Icon;
    bool isOverUI;
    public bool work = false;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    void Update()
    {
        isOverUI = EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(1) && selectOn == true && !isOverUI)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                if(GameManager.instance.groundMarker.GetComponent<Mark>().goWork == true)
                {
                    interact = Interact.work;
                    agent.SetDestination(hit.point);
                    transform.LookAt(hit.point);
                    work = true;
                }
                else
                {
                    interact = Interact.empty;
                    agent.SetDestination(hit.point);
                    transform.LookAt(hit.point);
                    work = false;
                }
            }
        }
        if (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetBool("walk", false);
        }
        else
            anim.SetBool("walk", true);
    }
    public void Skill1()
    {
        Debug.Log("Walka");
    }
    private void OnTriggerEnter(Collider other)
    {
        rb.freezeRotation = true;
        if (other.CompareTag("Build") && work == true && interact == Interact.work)
        {
            other.gameObject.GetComponent<BuildScr>().start = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        rb.freezeRotation = true;
        if (other.CompareTag("Build") && interact == Interact.work)
        {
            other.gameObject.GetComponent<BuildScr>().start = false;
            work = false;
            interact = Interact.empty;
        }
    }
}