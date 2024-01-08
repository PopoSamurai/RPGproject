using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Data;
using UnityEngine.SceneManagement;

public class GameManage : MonoBehaviour
{
    public int playerNum = 0;
    public GameObject[] players;
    public int score = 0;
    public Text scoreTxt, p1,p2,p3;
    public GameObject panelEnd;
    private void Start()
    {
        UpdateScore();
    }
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
            case 3:
                players[0].GetComponent<move>().activePlayer = false;
                players[1].GetComponent<move>().activePlayer = false;
                players[2].GetComponent<move>().activePlayer = false;
                break;
        }
        if(score == 3)
        {
            panelEnd.SetActive(true);
            playerNum = 3;
        }    
    }
    public void UpdateScore()
    {
        scoreTxt.text = score + " / 3";
    }
    public void changePlayer1()
    {
        playerNum = 0;
        p1.text = "ON";
        p2.text = "OFF";
        p3.text = "OFF";
    }
    public void changePlayer2()
    {
        playerNum = 1;
        p1.text = "OFF";
        p2.text = "ON";
        p3.text = "OFF";
    }
    public void changePlayer3()
    {
        playerNum = 2;
        p1.text = "OFF";
        p2.text = "OFF";
        p3.text = "ON";
    }
    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
