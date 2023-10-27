using BattleSystem;
using interactOn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InteractObjectToMove
{
    house,
    cave,
    backCave
}
public class MoveRooms : MonoBehaviour
{
    public InteractObjectToMove objectMap;
    public int objectID = 0;
    public GameObject InteractTag;
    public AudioSource sound;
    GameObject player;
    public bool set = false;
    public CameraFollow cam;
    public GameObject skip;
    public GameObject todestroy;
    public bool inHouse = false;
    public GameObject cave;
    public GameObject village;

    public GameObject[] transformPoints;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        switch (objectID)
        {
            case 1:
                if (set == true && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(czekaj());
                }
                break;
            case 2:
                if (set == true && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(czekaj1());
                }
                break;
            case 3:
                if (set == true && Input.GetKeyDown(KeyCode.E))
                {
                    village.SetActive(false);
                    cave.SetActive(true);
                    StartCoroutine(czekaj2());
                }
                break;
            case 4:
                if (set == true && Input.GetKeyDown(KeyCode.E))
                {
                    village.SetActive(true);
                    cave.SetActive(false);
                    StartCoroutine(czekaj3());
                }
                break;
        }
    }
    IEnumerator czekaj()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        yield return new WaitForSeconds(1f);
        cam.inHouse = 1;
        player.transform.position = transformPoints[1].transform.position;
        set = false;
        yield return new WaitForSeconds(1f);
        skip.SetActive(false);
        player.GetComponent<Movement>().move = true;
    }
    IEnumerator czekaj1()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        yield return new WaitForSeconds(1f);
        cam.inHouse = 0;
        player.transform.position = transformPoints[1].transform.position;
        set = false;
        yield return new WaitForSeconds(1f);
        skip.SetActive(false);
        player.GetComponent<Movement>().move = true;
    }
    IEnumerator czekaj2()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        yield return new WaitForSeconds(1f);
        cam.inHouse = 2;
        player.transform.position = transformPoints[1].transform.position;
        set = false;
        yield return new WaitForSeconds(1f);
        skip.SetActive(false);
        player.GetComponent<Movement>().move = true;
    }
    IEnumerator czekaj3()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        yield return new WaitForSeconds(1f);
        cam.inHouse = 0;
        player.transform.position = transformPoints[1].transform.position;
        set = false;
        yield return new WaitForSeconds(1f);
        skip.SetActive(false);
        player.GetComponent<Movement>().move = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && objectMap == InteractObjectToMove.house)
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
            Instantiate(InteractTag, transformPoints[0].transform);
            todestroy = GameObject.FindGameObjectWithTag("Interact");
            if (inHouse == false)
            {
                objectID = 1;
            }
            else
            {
                objectID = 2;
            }
        }
        if (other.CompareTag("Player") && objectMap == InteractObjectToMove.cave)
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
            Instantiate(InteractTag, transformPoints[0].transform);
            todestroy = GameObject.FindGameObjectWithTag("Interact");
            if (inHouse == false)
            {
                objectID = 3;
            }
            else
            {
                objectID = 4;
            }
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