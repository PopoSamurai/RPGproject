using BattleSystem;
using interactOn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InteractObjectToMove
{
    house,
    cave,
    forest
}
public class MoveRooms : MonoBehaviour
{
    public InteractObjectToMove objectMap;
    public int objectID = 1;
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
    public GameObject forest;
    public int tpNumber;
    GameObject musicManager;

    public GameObject[] transformPoints;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        musicManager = GameObject.FindGameObjectWithTag("Music");
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
                    village.SetActive(false);
                    cave.SetActive(true);
                    StartCoroutine(czekaj());
                }
                break;
            case 3:
                if (set == true && Input.GetKeyDown(KeyCode.E))
                {
                    village.SetActive(true);
                    cave.SetActive(false);
                    StartCoroutine(czekaj());
                }
                break;
        }
    }
    IEnumerator czekaj()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        if (objectMap == InteractObjectToMove.cave && inHouse == false)
        {
            musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().music[1];
            musicManager.GetComponent<AudioSource>().Play();
            player.GetComponent<Animator>().SetBool("battle", true);
        }
        else if(objectMap == InteractObjectToMove.cave && inHouse == true)
        {
            musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().music[0];
            musicManager.GetComponent<AudioSource>().Play();
            player.GetComponent<Animator>().SetBool("battle", false);
        }
        yield return new WaitForSeconds(1f);
        cam.inHouse = tpNumber;
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
            objectID = 1;
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
                objectID = 2;
            }
            else
            {
                objectID = 3;
            }
        }
        if (other.CompareTag("Player") && objectMap == InteractObjectToMove.forest)
        {
            set = true;
            sound.Play();
            if (inHouse == false)
            {
                musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().music[2];
                musicManager.GetComponent<AudioSource>().Play();
                village.SetActive(false);
                forest.SetActive(true);
                StartCoroutine(czekaj());
            }
            else
            {
                musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().music[0];
                musicManager.GetComponent<AudioSource>().Play();
                village.SetActive(true);
                forest.SetActive(false);
                StartCoroutine(czekaj());
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