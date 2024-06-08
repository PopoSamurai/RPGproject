using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
public class UnitScr : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;
    Animator anim;
    public bool selectOn = false;
    //
    public Sprite icon;
    public string buildName;
    public Sprite skill1Icon;
    bool isOverUI;
    void Start()
    {
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
                agent.SetDestination(hit.point);
                transform.LookAt(hit.point);
            }
        }
        if(agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
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
}