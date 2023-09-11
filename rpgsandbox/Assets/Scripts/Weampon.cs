using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weampon : MonoBehaviour
{
    public SpriteRenderer weamponSprite;
    public int id;
    public GameObject player;
    void Start()
    {
        id = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        switch(id)
        {
            case 0:
                //brak broni
                player.GetComponent<Movement>().haveSword = false;
                Debug.Log("Brak broni");
                break;
            case 1:
                player.GetComponent<Movement>().haveSword = true;
                break;
            case 2:
                player.GetComponent<Movement>().haveSword = true;
                break;
            case 3:
                player.GetComponent<Movement>().haveSword = true;
                break;
        }
    }
}
