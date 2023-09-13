using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class manaSys : MonoBehaviour
{
    public int mana = 10;
    public Image[] manaObj;

    void Update()
    {
        foreach (Image img in manaObj)
        {
            img.enabled = false;
        }
        for (int i = 0; i < mana; i++)
        {
            manaObj[i].enabled = true;
        }
    }
}