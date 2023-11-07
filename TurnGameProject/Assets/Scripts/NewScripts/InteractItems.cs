using BattleSystem;
using interactOn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;
public enum InteractObjects
{
    stone, 
    tree,
    fish
}
public class InteractItems : MonoBehaviour
{
    public InteractObjects objectMap;
    public GameObject InteractTag;
    public AudioSource sound;
    public AudioSource effectS;
    public GameObject claim;
    public int objectID = 0;
    //
    GameObject player;
    public bool set = false;
    public GameObject[] transformPoints;
    public GameObject todestroy;
    private void Start()
    {
        claim = GameObject.FindGameObjectWithTag("Claim");
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        switch (objectID)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Destroy(todestroy);
                    player.GetComponent<Movement>().move = false;
                    player.GetComponent<Animator>().SetBool("dig", true);
                    effectS.Play();
                    StartCoroutine(czekaj());
                }
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Destroy(todestroy);
                    player.GetComponent<Movement>().move = false;
                    player.GetComponent<Animator>().SetBool("fish", true);
                    effectS.Play();
                    StartCoroutine(czekaj2());
                }
                break;
        }
    }
    IEnumerator czekaj()
    {
        set = false;
        yield return new WaitForSeconds(3f); 
        claim.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.5f);
        effectS.Stop();
        player.GetComponent<Movement>().move = true;
        player.GetComponent<Animator>().SetBool("dig", false);
        Destroy(this.gameObject);
        set = false;
        objectID = 0;
    }
    IEnumerator czekaj2()
    {
        set = false;
        yield return new WaitForSeconds(3f);
        claim.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.5f);
        effectS.Stop();
        player.GetComponent<Movement>().move = true;
        player.GetComponent<Animator>().SetBool("fish", false);
        Destroy(todestroy);
        set = false;
        objectID = 0;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && objectMap == InteractObjects.stone)
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
            Instantiate(InteractTag, transformPoints[0].transform);
            todestroy = GameObject.FindGameObjectWithTag("Interact");
            objectID = 1;
        }
        if (other.CompareTag("Player") && objectMap == InteractObjects.fish)
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
            Instantiate(InteractTag, transformPoints[0].transform);
            todestroy = GameObject.FindGameObjectWithTag("Interact");
            objectID = 2;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(todestroy);
            set = false;
            objectID = 0;
        }
    }
}