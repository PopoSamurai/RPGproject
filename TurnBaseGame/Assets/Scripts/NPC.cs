using BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace interactOn
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
        public CameraFollow cam;
        public GameObject skip;
        public Transform inhouse;
        public Collider2D house;
        public GameObject dialogueWin;
        public Text dialogText;
        public GameObject upgradeWin;
        public int next = 0;
        public DialogueSystem dialogEvent;
        public int index;
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
                //playermove off
                player.GetComponent<Movement>().move = false;
                upgradeWin.SetActive(true);
            }
        }
        private void Update()
        {
            switch (objectID)
            {
                case 1:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        dialogueWin.SetActive(true);
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
                        Debug.Log("shop");
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
                objectID = 1;
            }
            if (other.CompareTag("Player") && objectMap == InteractObject.shop)
            {
                set = true;
                sound.Play();
                InteractTag.SetActive(true);
                objectID = 2;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                InteractTag.SetActive(false);
                set = false;
                dialogueWin.SetActive(false);
                index = 0;
                objectID = 0;
            }
        }
    }
}