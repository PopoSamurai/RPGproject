using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    private Vector3 target;
    NavMeshAgent agent;
    Animator anim;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        SetTargetPos();
        SetAgentPos();
    }
    public void SetTargetPos()
    {
        if(Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    public void SetAgentPos()
    {
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }
}
