using System.Transactions;
using UnityEngine;
using UnityEngine.AI;
public class UnitScr : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;
    Animator anim;
    public bool selectOn = false;
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
        if (Input.GetMouseButtonDown(1) && selectOn == true)
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
}