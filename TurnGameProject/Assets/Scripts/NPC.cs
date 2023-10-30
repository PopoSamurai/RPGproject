using BattleSystem;
using interactOn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public enum InteractObject
    {
        npc,
        shop
    }
    public class NPC : MonoBehaviour
    {
        public InteractObject objectMap;
        public GameObject InteractTag;
        public AudioSource sound;
        public int objectID = 0;
        //
        GameObject player;
        public bool set = false;
        public GameObject dialogueWin;
        public Text nameText;
        public Text dialogText;
        public GameObject upgradeWin;
        public int next = 0;
        public DialogueSystem dialogEvent;
        public int index;
        public GameObject icons;

        public GameObject[] transformPoints;
        public GameObject todestroy;

        public Transform textPos;
        public GameObject prefabText;
        public string[] txtExit;
        int number = 0;
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        void NextLine()
        {
            if (index < dialogEvent.lines.Length - 1)
            {
                index++;
            }
            else
            {
                dialogueWin.SetActive(false);
                icons.SetActive(true);
                upgradeWin.SetActive(true);
                player.GetComponent<Movement>().move = false;
            }
        }
        private void Update()
        {
            switch (objectID)
            {
                case 1:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        icons.SetActive(false);
                        dialogueWin.SetActive(true);
                        nameText.text = dialogEvent.nameNPC;
                        dialogText.text = dialogEvent.lines[index];
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            dialogText.text = dialogEvent.lines[index];
                            NextLine();
                        }
                    }
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        icons.SetActive(true);
                        upgradeWin.SetActive(true);
                        player.GetComponent<Movement>().move = false;
                    }
                    break;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && objectMap == InteractObject.npc)
            {
                set = true;
                sound.Play();
                InteractTag.SetActive(true);
                Instantiate(InteractTag, transformPoints[0].transform);
                todestroy = GameObject.FindGameObjectWithTag("Interact");
                objectID = 1;
            }
            if (other.CompareTag("Player") && objectMap == InteractObject.shop)
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
            number++;
            if (number >= 4)
                number = 0;

            if (other.CompareTag("Player"))
            {
                prefabText.GetComponent<Text>().text = txtExit[number];
                Instantiate(prefabText, textPos);
                Destroy(todestroy);
                set = false;
                dialogueWin.SetActive(false);
                index = 0;
                objectID = 0;

            }
        }
    }
}