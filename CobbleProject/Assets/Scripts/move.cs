using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour
{
    public bool activePlayer;
    NavMeshAgent agent;
    public float predkosc, zwrotnosc, wytrzymalosc;
    float radius = 3f;
    public GameManage gameManage;
    public int number;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        predkosc = Random.Range(2f, 8f);
        zwrotnosc = Random.Range(0.5f, 5f);
        wytrzymalosc = Random.Range(0.5f, 5f);
        agent.speed = predkosc;
    }

    void Update()
    {
        SetTargetPos();
        if (activePlayer == false)
        {
            number = gameManage.playerNum;
            Vector3 movePos = gameManage.players[number].transform.position;
            movePos = Vector3.MoveTowards(movePos, transform.position, radius);
            agent.SetDestination(movePos);
        }
    }
    public void SetTargetPos()
    {
        if (Input.GetMouseButtonDown(0) && activePlayer == true)
        {
            Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(mousePos, out var hitInfo))
            {
                agent.SetDestination(hitInfo.point);
            }
        }
    }
}