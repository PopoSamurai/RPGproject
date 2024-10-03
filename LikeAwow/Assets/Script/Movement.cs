using System;
using UnityEngine;
public class Movement : MonoBehaviour
{
    public GameObject[] poses; // 1 - down / 2 - up / 3 - left / 4 - right
    public int activeIndex = 0;
    int index = 0;
    void Update()
    {
        for (int i = 0; i < poses.Length; i++)
        {
            if (i == index)
            {
                poses[i].SetActive(true);
            }
            else
            {
                poses[i].SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
            index = 0;
        if (Input.GetKeyDown(KeyCode.W))
            index = 1;
        if (Input.GetKeyDown(KeyCode.A))
            index = 2;
        if (Input.GetKeyDown(KeyCode.D))
            index = 3;
    }
}