using BattleSystem;
using interactOn;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum InteractObject
{
    house,
    cave,
    backCave
}
public class Teleport : MonoBehaviour
{
    public InteractObject objectMap;
    public int objectID = 0;
    public GameObject InteractTag;
    public AudioSource sound;
    GameObject player;
    public bool set = false;
    public CameraFollow cam;
    public GameObject skip;
    public Transform inHouse;
    public Collider2D house;
    public int check = 0;
    public Converter gamem;
    private void Start()
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
                    StartCoroutine(czekajNaTp());
                }
                break;
            case 3:
                if (set == true && Input.GetKeyDown(KeyCode.E))
                {
                    gamem.GetComponent<Converter>().spawnPoint = 1;
                    StartCoroutine(czekajNaTpBack());
                }
                break;
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
    IEnumerator czekajNaTp()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Cave");
    }
    IEnumerator czekajNaTpBack()
    {
        player.GetComponent<Movement>().move = false;
        skip.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Village");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && objectMap == InteractObject.house)
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
            objectID = 1;
        }
        if (other.CompareTag("Player") && objectMap == InteractObject.cave)
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
            objectID = 2;
        }
        if (other.CompareTag("Player") && objectMap == InteractObject.backCave)
        {
            set = true;
            sound.Play();
            InteractTag.SetActive(true);
            objectID = 3;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractTag.SetActive(false);
            set = false;
            objectID = 0;
        }
    }
}
