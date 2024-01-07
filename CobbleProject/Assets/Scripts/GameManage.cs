using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManage : MonoBehaviour
{
    public int playerNum = 0;
    public GameObject[] players;
    void Update()
    {
        switch (playerNum) 
        {
            case 0:
                players[0].GetComponent<move>().activePlayer = true;
                players[1].GetComponent<move>().activePlayer = false;
                players[2].GetComponent<move>().activePlayer = false;
                break;
            case 1:
                players[0].GetComponent<move>().activePlayer = false;
                players[1].GetComponent<move>().activePlayer = true;
                players[2].GetComponent<move>().activePlayer = false;
                break;
            case 2:
                players[0].GetComponent<move>().activePlayer = false;
                players[1].GetComponent<move>().activePlayer = false;
                players[2].GetComponent<move>().activePlayer = true;
                break;
        }
    }
    public void changePlayer1()
    {
        playerNum = 0;
    }
    public void changePlayer2()
    {
        playerNum = 1;
    }
    public void changePlayer3()
    {
        playerNum = 2;
    }
}
