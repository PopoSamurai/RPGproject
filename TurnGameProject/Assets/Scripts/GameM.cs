using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace BattleSystem
{
    public class GameM : MonoBehaviour
    {
        public Teammates playerTeam;
        public Converter enemyTeam;
        public Transform[] spawn;
        public Converter spawner;
        private GameObject player;
        public Vector3 startPos;
        public SavePlayer save;
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            switch (spawner.GetComponent<Converter>().spawnPoint)
            {
                case 0:
                    startPos = spawn[0].position;
                    player.transform.position = startPos;
                    break;
                case 1:
                    startPos = spawn[1].position;
                    player.transform.position = startPos;
                    break;
                case 2:
                    startPos = save.savePos;
                    player.transform.position = startPos;
                    break;
            }
        }
        private void Update()
        {
            playerTeam.playernum = playerTeam.allCharacters.Count();
            foreach(GameObject element in playerTeam.allCharacters)
            {
                if(element == null)
                {
                    playerTeam.playernum--;
                }
            }
            enemyTeam.enemynum = enemyTeam.EnemyTeam.Count();
            foreach (GameObject element2 in enemyTeam.EnemyTeam)
            {
                if (element2 == null)
                {
                    enemyTeam.enemynum--;
                }
            }
        }
    }
}