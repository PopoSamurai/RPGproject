using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class Converter : MonoBehaviour
    {
        public GameObject[] EnemyTeam;
        public int enemynum = 0;
        public int spawnPoint = 0;
        private void Start()
        {
            enemynum = 0;
            Destroy(EnemyTeam[0]);
            Destroy(EnemyTeam[1]);
            Destroy(EnemyTeam[2]);
            Destroy(EnemyTeam[3]);
        }
    }
}