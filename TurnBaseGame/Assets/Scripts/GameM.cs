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