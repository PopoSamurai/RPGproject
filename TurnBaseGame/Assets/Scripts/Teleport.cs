using BattleSystem;
using interactOn;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject InteractTag;
    public AudioSource sound;
    GameObject player;
    public bool set = false;
    public CameraFollow cam;
    public GameObject skip;
    public Transform inHouse;
    public Collider2D house;
    public int check = 0;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(set == true && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(czekaj());
        }
    }
    IEnumerator czekaj()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        yield return new WaitForSeconds(1f);
        cam.inHouse = check;
        house.enabled = false;
        player.transform.position = inHouse.position;
        set = false;
        yield return new WaitForSeconds(1f);
        house.enabled = true;
        skip.SetActive(false);
        player.GetComponent<Movement>().move = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractTag.SetActive(false);
            set = false;

        }
    }
}
