using BattleSystem;
using interactOn;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleSystem
{
    public class BattleAttack : MonoBehaviour
    {
        public GameObject[] EnemyTeam;
        GameObject player;
        public GameObject skip;
        public Converter enemyback;
        public SavePlayer save;
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        IEnumerator czekaj()
        {
            player.GetComponent<Movement>().move = false;
            skip.SetActive(true);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("battle");
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                save.savePos = this.gameObject.transform.position;
                enemyback.spawnPoint = 2;
                enemyback.GetComponent<Converter>().EnemyTeam[0] = EnemyTeam[0];
                enemyback.GetComponent<Converter>().EnemyTeam[1] = EnemyTeam[1];
                enemyback.GetComponent<Converter>().EnemyTeam[2] = EnemyTeam[2];
                enemyback.GetComponent<Converter>().EnemyTeam[3] = EnemyTeam[3];
                StartCoroutine(czekaj());
            }
        }
    }
}