using interactOn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class BattleOn : MonoBehaviour
    {
        public GameObject battleWindow;
        public GameObject caveWin;
        private GameObject player;
        GameObject musicManager;
        public GameObject gui;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            musicManager = GameObject.FindGameObjectWithTag("Music");
            battleWindow.SetActive(false);
        }

        void Update()
        {

        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                player.GetComponent<Movement>().MoveOff();
                battleWindow.SetActive(true);
                caveWin.SetActive(false);
                musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().music[3];
                musicManager.GetComponent<AudioSource>().Play();
                gui.SetActive(false);
            }
        }
    }
}
