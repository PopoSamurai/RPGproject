using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        SetTargetPos();
    }
    public void SetTargetPos()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mousePos, out var hitInfo))
            {
                agent.SetDestination(hitInfo.point);
            }
        }

        if(agent.remainingDistance > agent.stoppingDistance)
        {
            Move(agent.desiredVelocity);
        }
        else
        {
            Move(Vector3.zero);
        }
    }
    public void Move(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);

        if (move != Vector3.zero)
        {
            anim.SetBool("move", true);
        }
        else
            anim.SetBool("move", false);
    }
}