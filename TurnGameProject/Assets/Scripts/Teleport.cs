using BattleSystem;
using interactOn;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace BattleSystem
{
    public enum InteractObjectToMove
    {
        house,
        cave,
        backCave
    }
    public class Teleport : MonoBehaviour
    {
        public InteractObjectToMove objectMap;
        public int objectID = 0;
        public GameObject InteractTag;
        public AudioSource sound;
        GameObject player;
        public bool set = false;
        public CameraFollow cam;
        public GameObject skip;
        public Transform inHouse;
        public Transform inCave;
        public Collider2D house;
        public int check = 0;
        public Converter gamem;
        public GameObject caveObjects;
        public GameObject villageObjects;
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
                        StartCoroutine(czekajNaTpBack());
                    }
                    break;
            }
        }
        IEnumerator czekaj()
        {
            player.GetComponent<Animator>().SetBool("battle", false);
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
            player.GetComponent<Animator>().SetBool("battle", true);
            skip.SetActive(true); 
            cam.inHouse = check;
            player.transform.position = inHouse.position;
            yield return new WaitForSeconds(2f);
            //SceneManager.LoadScene("Cave");
            caveObjects.SetActive(true);
            villageObjects.SetActive(false);
            player.transform.position = inCave.position;
            player.GetComponent<Movement>().move = true;
        }
        IEnumerator czekajNaTpBack()
        {
            player.GetComponent<Movement>().move = false;
            skip.SetActive(true);
            gamem.GetComponent<Converter>().spawnPoint = 1;
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Village");
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && objectMap == InteractObjectToMove.house)
            {
                set = true;
                sound.Play();
                InteractTag.SetActive(true);
                objectID = 1;
            }
            if (other.CompareTag("Player") && objectMap == InteractObjectToMove.cave)
            {
                set = true;
                sound.Play();
                InteractTag.SetActive(true);
                objectID = 2;
            }
            if (other.CompareTag("Player") && objectMap == InteractObjectToMove.backCave)
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
}