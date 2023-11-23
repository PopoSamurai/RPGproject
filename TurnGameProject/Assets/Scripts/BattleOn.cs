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
        GameObject player;
        GameObject musicManager;
        public GameObject gui;
        public GameObject skip;
        GameObject walka;
        void Start()
        {
            walka = GameObject.FindGameObjectWithTag("GameM");
            player = GameObject.FindGameObjectWithTag("Player");
            musicManager = GameObject.FindGameObjectWithTag("Music");
            battleWindow.SetActive(false);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                player.GetComponent<Movement>().MoveOff();
                caveWin.SetActive(false);
                musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().music[3];
                musicManager.GetComponent<AudioSource>().Play();
                StartCoroutine(czekajna());
            }
        }
        public IEnumerator czekajna()
        {
            skip.SetActive(true);
            yield return new WaitForSeconds(1f);
            skip.SetActive(false);
            gui.SetActive(false);
            battleWindow.SetActive(true);
            walka.GetComponent<Fight>().battleOn();
            Destroy(this.gameObject);
        }
    }
}