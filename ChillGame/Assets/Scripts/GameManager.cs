using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] panels;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].SetActive(false);
                ResumeGame();
            }
        }
        if (Input.GetKey(KeyCode.I))
        {
            panels[0].SetActive(true);
            PauseGame();
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        Movement.move = false;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        Movement.move = true;
    }
}
