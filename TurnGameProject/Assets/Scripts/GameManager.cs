using BattleSystem;
using interactOn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class GameManager : MonoBehaviour
    {
        public GameObject battleWindow;
        public GameObject caveWin;
        GameObject player;
        GameObject musicManager;
        public GameObject gui;
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            musicManager = GameObject.FindGameObjectWithTag("Music");
        }
        public void EndBattle()
        {
            gui.SetActive(true);
            battleWindow.SetActive(false);
            caveWin.SetActive(true);
            player.GetComponent<Movement>().MoveOn();
            musicManager.GetComponent<AudioSource>().clip = musicManager.GetComponent<MusicManager>().music[1];
            musicManager.GetComponent<AudioSource>().Play();
        }
    }
}
