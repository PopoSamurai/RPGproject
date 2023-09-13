using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthSys : MonoBehaviour
{
    public int health = 10;
    public Image[] hearts;

    void Update()
    {
        foreach (Image img in hearts)
        {
            img.enabled = false;
        }
        for (int i = 0; i < health; i++)
        {
            hearts[i].enabled = true;
        }
    }
}