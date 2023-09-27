using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public enum BattleState
    {
        start,
        playerTurn,
        enemyTurn,
        won,
        lost
    }
    public class Battle : MonoBehaviour
    {
        public GameObject[] EnemyTeam;
        //pos players
        public Transform playerFirstPos;
        public Transform playerTwoPos;
        public Transform playerThreePos;
        public Transform playerFourPos;
        //pos enemy
        public Transform enemyFirstPos;
        public Transform enemyTwoPos;
        public Transform enemyThreePos;
        public Transform enemyFourPos;

        public Text dialogueText;
        CharacterClass playerClass;
        CharacterClass playerClass2;
        CharacterClass playerClass3;
        CharacterClass playerClass4;
        CharacterClass enemyClass;
        CharacterClass enemyClass2;
        CharacterClass enemyClass3;
        CharacterClass enemyClass4;

        public BattleHUD playerHud;
        public BattleHUD playerHud2;
        public BattleHUD playerHud3;
        public BattleHUD playerHud4;
        public BattleHUD enemyHud;
        public BattleHUD enemyHud2;
        public BattleHUD enemyHud3;
        public BattleHUD enemyHud4;
        public int playersNum = 0;
        public int enemyNum = 0;
        public BattleState state;
        public Vector3 offset;
        public Vector3 offsetE;
        public GameObject[] effect;
        public AudioSource emptymana;
        public GameObject[] uiPanel;
        //
        public Teammates team;
        public Converter enemyback;
        void Start()
        {
            playersNum = team.playernum;
            enemyNum = enemyback.enemynum;
            EnemyTeam[0] = enemyback.GetComponent<Converter>().EnemyTeam[0];
            EnemyTeam[1] = enemyback.GetComponent<Converter>().EnemyTeam[1];
            EnemyTeam[2] = enemyback.GetComponent<Converter>().EnemyTeam[2];
            EnemyTeam[3] = enemyback.GetComponent<Converter>().EnemyTeam[3];

            state = BattleState.start;
            TeammateLoad();
            EnemyLoad();
            StartCoroutine(stetupBattle());
        }
        public void TeammateLoad()
        {
            switch(playersNum)
            {
                case 1:
                    playerHud.gameObject.SetActive(true);
                    playerHud2.gameObject.SetActive(false);
                    playerHud3.gameObject.SetActive(false);
                    playerHud4.gameObject.SetActive(false);
                    GameObject player1Go = Instantiate(team.allCharacters[0], playerFirstPos.position, playerFirstPos.rotation);
                    playerClass = player1Go.GetComponent<CharacterClass>();
                    playerHud.setHud(playerClass);
                    break;
                case 2:
                    playerHud.gameObject.SetActive(true);
                    playerHud2.gameObject.SetActive(true);
                    playerHud3.gameObject.SetActive(false);
                    playerHud4.gameObject.SetActive(false);
                    GameObject player1aGo = Instantiate(team.allCharacters[0], playerFirstPos.position, playerFirstPos.rotation);
                    playerClass = player1aGo.GetComponent<CharacterClass>();
                    playerHud.setHud(playerClass);
                    GameObject player2Go = Instantiate(team.allCharacters[1], playerTwoPos.position, playerTwoPos.rotation);
                    playerClass2 = player2Go.GetComponent<CharacterClass>();
                    playerHud2.setHud(playerClass2);
                    break;
                case 3:
                    playerHud2.gameObject.SetActive(true);
                    playerHud3.gameObject.SetActive(true);
                    playerHud4.gameObject.SetActive(true);
                    playerHud.gameObject.SetActive(false);
                    GameObject player1bGo = Instantiate(team.allCharacters[0], playerFirstPos.position, playerFirstPos.rotation);
                    playerClass = player1bGo.GetComponent<CharacterClass>();
                    playerHud.setHud(playerClass);
                    GameObject player2aGo = Instantiate(team.allCharacters[1], playerTwoPos.position, playerTwoPos.rotation);
                    playerClass2 = player2aGo.GetComponent<CharacterClass>();
                    playerHud2.setHud(playerClass2);
                    GameObject player3Go = Instantiate(team.allCharacters[2], playerThreePos.position, playerThreePos.rotation);
                    playerClass3 = player3Go.GetComponent<CharacterClass>();
                    playerHud3.setHud(playerClass3);
                    break;
                case 4:
                    playerHud.gameObject.SetActive(true);
                    playerHud2.gameObject.SetActive(true);
                    playerHud3.gameObject.SetActive(true);
                    playerHud4.gameObject.SetActive(false);
                    GameObject player1cGo = Instantiate(team.allCharacters[0], playerFirstPos.position, playerFirstPos.rotation);
                    playerClass = player1cGo.GetComponent<CharacterClass>();
                    playerHud.setHud(playerClass);
                    GameObject player2bGo = Instantiate(team.allCharacters[1], playerTwoPos.position, playerTwoPos.rotation);
                    playerClass2 = player2bGo.GetComponent<CharacterClass>();
                    playerHud2.setHud(playerClass2);
                    GameObject player3aGo = Instantiate(team.allCharacters[2], playerThreePos.position, playerThreePos.rotation);
                    playerClass3 = player3aGo.GetComponent<CharacterClass>();
                    playerHud3.setHud(playerClass3);
                    GameObject player4Go = Instantiate(team.allCharacters[3], playerFourPos.position, playerFourPos.rotation);
                    playerClass4 = player4Go.GetComponent<CharacterClass>();
                    playerHud4.setHud(playerClass4);
                    break;
            }
        }
        public void EnemyLoad()
        {
            switch (enemyNum)
            {
                case 1:
                    enemyHud.gameObject.SetActive(true);
                    enemyHud2.gameObject.SetActive(false);
                    enemyHud3.gameObject.SetActive(false);
                    enemyHud4.gameObject.SetActive(false);
                    GameObject enemyGo = Instantiate(EnemyTeam[0], enemyFirstPos.position, enemyFirstPos.rotation);
                    enemyClass = enemyGo.GetComponent<CharacterClass>();
                    enemyClass.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemyGo.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud.setHud(enemyClass);
                    break;
                case 2:
                    enemyHud.gameObject.SetActive(true);
                    enemyHud2.gameObject.SetActive(true);
                    enemyHud3.gameObject.SetActive(false);
                    enemyHud4.gameObject.SetActive(false);
                    GameObject enemy1Go = Instantiate(EnemyTeam[0], enemyFirstPos.position, enemyFirstPos.rotation);
                    enemyClass = enemy1Go.GetComponent<CharacterClass>();
                    enemyClass.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy1Go.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud.setHud(enemyClass);
                    GameObject enemy2Go = Instantiate(EnemyTeam[1], enemyTwoPos.position, enemyTwoPos.rotation);
                    enemyClass2 = enemy2Go.GetComponent<CharacterClass>();
                    enemyClass2.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy2Go.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud2.setHud(enemyClass2);
                    break;
                case 3:
                    enemyHud.gameObject.SetActive(true);
                    enemyHud2.gameObject.SetActive(true);
                    enemyHud3.gameObject.SetActive(true);
                    enemyHud4.gameObject.SetActive(false);
                    GameObject enemy1aGo = Instantiate(EnemyTeam[0], enemyFirstPos.position, enemyFirstPos.rotation);
                    enemyClass = enemy1aGo.GetComponent<CharacterClass>();
                    enemyClass.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy1aGo.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud.setHud(enemyClass);
                    GameObject enemy2aGo = Instantiate(EnemyTeam[1], enemyTwoPos.position, enemyTwoPos.rotation);
                    enemyClass2 = enemy2aGo.GetComponent<CharacterClass>();
                    enemyClass2.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy2aGo.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud2.setHud(enemyClass2);
                    GameObject enemy3Go = Instantiate(EnemyTeam[2], enemyThreePos.position, enemyThreePos.rotation);
                    enemyClass3 = enemy3Go.GetComponent<CharacterClass>();
                    enemyClass3.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy3Go.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud3.setHud(enemyClass3);
                    break;
                case 4:
                    enemyHud.gameObject.SetActive(true);
                    enemyHud2.gameObject.SetActive(true);
                    enemyHud3.gameObject.SetActive(true);
                    enemyHud4.gameObject.SetActive(true);
                    GameObject enemy1bGo = Instantiate(EnemyTeam[0], enemyFirstPos.position, enemyFirstPos.rotation);
                    enemyClass = enemy1bGo.GetComponent<CharacterClass>();
                    enemyClass.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy1bGo.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud.setHud(enemyClass);
                    GameObject enemy2bGo = Instantiate(EnemyTeam[1], enemyTwoPos.position, enemyTwoPos.rotation);
                    enemyClass2 = enemy2bGo.GetComponent<CharacterClass>();
                    enemyClass2.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy2bGo.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud2.setHud(enemyClass2);
                    GameObject enemy3aGo = Instantiate(EnemyTeam[2], enemyThreePos.position, enemyThreePos.rotation);
                    enemyClass3 = enemy3aGo.GetComponent<CharacterClass>();
                    enemyClass3.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy3aGo.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud3.setHud(enemyClass3);
                    GameObject enemy4Go = Instantiate(EnemyTeam[3], enemyFourPos.position, enemyFourPos.rotation);
                    enemyClass4 = enemy4Go.GetComponent<CharacterClass>();
                    enemyClass4.GetComponent<CharacterClass>().com = pOrE.enemy;
                    enemy4Go.transform.localScale = new Vector3(1, 1, 0);
                    enemyHud4.setHud(enemyClass4);
                    break;
            }
        }
        public IEnumerator stetupBattle()
        {
            dialogueText.text = "Pojawiaj¹ siê wrogowie... ";
            yield return new WaitForSeconds(2f);

            state = BattleState.playerTurn;
            PlayerTurn();
        }

        public void PlayerTurn()
        {
            dialogueText.text = "Twoja tura: ";
            playersNum = Random.Range(1,3);
            CheckTarget1();
            switch (playersNum)
            {
                case 1:
                    foreach (var obj in playerClass.skills)
                    {
                        Instantiate(obj, GameObject.FindGameObjectWithTag("Canvas").transform);
                    }
                    playerHud.color.SetActive(true);
                    break;
                case 2:
                    foreach (var obj in playerClass2.skills)
                    {
                        Instantiate(obj, GameObject.FindGameObjectWithTag("Canvas").transform);
                    }
                    playerHud2.color.SetActive(true);
                    break;
            }
        }
        //attack
        public void AttackButton()
        {
            StartCoroutine(czekajNaCol());
            if (state != BattleState.playerTurn)
            {
                return;
            }
            StartCoroutine(PlayerAttack());
        }
        IEnumerator czekajNaCol()
        {
            yield return new WaitForSeconds(1f);
            foreach (Transform child in GameObject.FindGameObjectWithTag("Canvas").transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        IEnumerator czekajNaCol2()
        {
            yield return new WaitForSeconds(0.5f);
            foreach (Transform child in GameObject.FindGameObjectWithTag("Canvas").transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            yield return new WaitForSeconds(1f);

            switch (playersNum)
            {
                case 1:
                    foreach (var obj in playerClass.skills)
                    {
                        Instantiate(obj, GameObject.FindGameObjectWithTag("Canvas").transform);
                    }
                    break;
                case 2:
                    foreach (var obj in playerClass2.skills)
                    {
                        Instantiate(obj, GameObject.FindGameObjectWithTag("Canvas").transform);
                    }
                    break;
            }
        }
        //heal
        public void HealButton()
        {
            if (state != BattleState.playerTurn)
            {
                return;
            }
            switch (playersNum)
            {
                case 1:
                    if (playerClass.currentSp >= 10 && playerClass.currentHp < playerClass.health)
                    {
                        StartCoroutine(czekajNaCol());
                        StartCoroutine(PlayerHeal());
                    }
                    else
                    {
                        emptymana.Play();
                        StartCoroutine(czekajNaCol2());
                    }
                    break;
                case 2:
                    if (playerClass2.currentSp >= 10 && playerClass2.currentHp < playerClass2.health)
                    {
                        StartCoroutine(czekajNaCol());
                        StartCoroutine(PlayerHeal());
                    }
                    else
                    {
                        emptymana.Play();
                        StartCoroutine(czekajNaCol2());
                    }
                    break;
            }
        }
        IEnumerator PlayerHeal()
        {
            switch (playersNum)
            {
                case 1:
                    playerClass.Heal(5, 10);
                    playerHud.SetHp(playerClass.currentHp);
                    playerHud.SetSp(playerClass.currentSp);
                    dialogueText.text = "Czujesz przyp³yw zdrowia!";
                    disableColors();
                    yield return new WaitForSeconds(2f);
                    break;
                case 2:
                    playerClass2.Heal(5, 10);
                    playerHud2.SetHp(playerClass2.currentHp);
                    playerHud2.SetSp(playerClass2.currentSp);
                    dialogueText.text = "Czujesz przyp³yw zdrowia!";
                    disableColors();
                    yield return new WaitForSeconds(2f);
                    break;
            }
            state = BattleState.enemyTurn;
            StartCoroutine(EnemyTour());
        }
        public void CheckTarget1()
        {
            enemyNum = 1;
            enemyHud.color.SetActive(true);
            enemyHud2.color.SetActive(false);
        }
        public void CheckTarget2()
        {
            enemyNum = 2;
            enemyHud2.color.SetActive(true);
            enemyHud.color.SetActive(false);
        }

        public void disableColors()
        {
            playerHud.color.SetActive(false);
            playerHud2.color.SetActive(false);
            enemyHud2.color.SetActive(false);
            enemyHud.color.SetActive(false);
        }

        public IEnumerator PlayerAttack()
        {
            switch (playersNum)
            {
                case 1:
                    playerClass.AttackCo();
                    switch (enemyNum)
                    {
                        case 1:
                            bool isDeadE1 = enemyClass.TakeDamage(playerClass.damage);
                            if (playerClass.classChar != CharClass.mage)
                            {
                                playerClass.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                playerClass.transform.position = enemyFirstPos.transform.position + offset;
                                Instantiate(effect[2], enemyFirstPos);
                            }
                            else
                            {
                                Instantiate(effect[0], enemyFirstPos);
                            }
                            enemyHud.SetHp(enemyClass.currentHp);
                            enemyHud.SetSp(enemyClass.currentSp);
                            dialogueText.text = "Atak siê uda³.";
                            disableColors();

                            yield return new WaitForSeconds(2f);
                            team.allCharacters[0].transform.position = playerFirstPos.position;
                            playerClass.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadE1)
                            {
                                state = BattleState.won;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.enemyTurn;
                                StartCoroutine(EnemyTour());
                            }
                            break;
                        case 2:
                            bool isDeadE2 = enemyClass2.TakeDamage(playerClass.damage);
                            if (playerClass.classChar != CharClass.mage)
                            {
                                playerClass.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                playerClass.transform.position = enemyTwoPos.transform.position + offset;
                                Instantiate(effect[2], enemyTwoPos);
                            }
                            else
                            {
                                Instantiate(effect[0], enemyTwoPos);
                            }
                            enemyHud2.SetHp(enemyClass2.currentHp);
                            enemyHud2.SetSp(enemyClass2.currentSp);
                            dialogueText.text = "Atak siê uda³.";

                            disableColors();
                            yield return new WaitForSeconds(2f);
                            team.allCharacters[0].transform.position = playerFirstPos.position;
                            playerClass.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadE2)
                            {
                                state = BattleState.won;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.enemyTurn;
                                StartCoroutine(EnemyTour());
                            }
                            break;
                    }
                    break;
                case 2:
                    playerClass2.AttackCo();
                    switch (enemyNum)
                    {
                        case 1:
                            bool isDeadE1 = enemyClass.TakeDamage(playerClass2.damage);
                            if (playerClass2.classChar != CharClass.mage)
                            {
                                playerClass2.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                playerClass2.transform.position = enemyFirstPos.transform.position + offset;
                                Instantiate(effect[1], enemyFirstPos);
                            }
                            else
                            {
                                Instantiate(effect[0], enemyFirstPos);
                            }
                            enemyHud.SetHp(enemyClass.currentHp);
                            enemyHud.SetSp(enemyClass.currentSp);
                            dialogueText.text = "Atak siê uda³.";
                            disableColors();

                            yield return new WaitForSeconds(2f);
                            team.allCharacters[1].transform.position = playerTwoPos.position;
                            playerClass2.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadE1)
                            {
                                state = BattleState.won;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.enemyTurn;
                                StartCoroutine(EnemyTour());
                            }
                            break;
                        case 2:
                            bool isDeadE2 = enemyClass2.TakeDamage(playerClass2.damage);
                            if (playerClass2.classChar != CharClass.mage)
                            {
                                playerClass2.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                playerClass2.transform.position = enemyTwoPos.transform.position + offset;
                                Instantiate(effect[1], enemyTwoPos);
                            }
                            else
                            {
                                Instantiate(effect[0], enemyTwoPos);
                            }
                            enemyHud2.SetHp(enemyClass2.currentHp);
                            enemyHud2.SetSp(enemyClass2.currentSp);
                            dialogueText.text = "Atak siê uda³.";
                            disableColors();

                            yield return new WaitForSeconds(2f);
                            team.allCharacters[1].transform.position = playerTwoPos.position;
                            playerClass2.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadE2)
                            {
                                state = BattleState.won;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.enemyTurn;
                                StartCoroutine(EnemyTour());
                            }
                            break;
                    }
                    break;
            }
        }
        public IEnumerator EnemyTour()
        {
            enemyNum = Random.Range(1, 3);
            playersNum = Random.Range(1, 3);
            switch (enemyNum)
            {
                case 1:
                    enemyClass.AttackCo();
                    switch(playersNum)
                    {
                        case 1:
                            bool isDeadP1 = playerClass.TakeDamage(enemyClass.damage);
                            if (enemyClass.classChar != CharClass.mage)
                            {
                                enemyClass.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                enemyClass.transform.position = playerFirstPos.transform.position + offsetE;
                                Instantiate(effect[1], playerFirstPos);
                            }
                            else
                            {
                                Instantiate(effect[0], playerFirstPos);
                            }
                            playerHud.SetHp(playerClass.currentHp);
                            playerHud.SetSp(playerClass.currentSp);
                            dialogueText.text = enemyClass.nameP + " atakuje..";
                            yield return new WaitForSeconds(1f);
                            EnemyTeam[0].transform.position = enemyFirstPos.position;
                            enemyClass.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadP1)
                            {
                                state = BattleState.lost;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.playerTurn;
                                PlayerTurn();
                            }
                            break;
                        case 2:
                            bool isDeadP2 = playerClass2.TakeDamage(enemyClass.damage);
                            if (enemyClass.classChar != CharClass.mage)
                            {
                                enemyClass.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                enemyClass.transform.position = playerTwoPos.transform.position + offsetE;
                                Instantiate(effect[1], playerTwoPos);
                            }
                            else
                            {
                                Instantiate(effect[0], playerTwoPos);
                            }
                            playerHud2.SetHp(playerClass2.currentHp);
                            playerHud2.SetSp(playerClass2.currentSp);
                            dialogueText.text = enemyClass.nameP + " atakuje..";
                            yield return new WaitForSeconds(1f);
                            EnemyTeam[0].transform.position = enemyFirstPos.position;
                            enemyClass.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadP2)
                            {
                                state = BattleState.lost;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.playerTurn;
                                PlayerTurn();
                            }
                            break;
                    }
                    break;
                case 2:
                    enemyClass2.AttackCo();
                    switch (playersNum)
                    {
                        case 1:
                            bool isDeadP1 = playerClass.TakeDamage(enemyClass2.damage);
                            if (enemyClass2.classChar != CharClass.mage)
                            {
                                enemyClass2.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                enemyClass2.transform.position = playerFirstPos.transform.position + offsetE;
                                Instantiate(effect[1], playerFirstPos);
                            }
                            else
                            {
                                Instantiate(effect[0], playerFirstPos);
                            }
                            playerHud.SetHp(playerClass.currentHp);
                            playerHud.SetSp(playerClass.currentSp);
                            dialogueText.text = enemyClass2.nameP + " atakuje..";
                            yield return new WaitForSeconds(1f);
                            EnemyTeam[1].transform.position = enemyTwoPos.position;
                            enemyClass2.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadP1)
                            {
                                state = BattleState.lost;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.playerTurn;
                                PlayerTurn();
                            }
                            break;
                        case 2:
                            bool isDeadP2 = playerClass2.TakeDamage(enemyClass2.damage);
                            if (enemyClass2.classChar != CharClass.mage)
                            {
                                enemyClass2.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                enemyClass2.transform.position = playerTwoPos.transform.position + offsetE;
                                Instantiate(effect[1], playerTwoPos);
                            }
                            else
                            {
                                Instantiate(effect[0], playerTwoPos);
                            }
                            playerHud2.SetHp(playerClass2.currentHp);
                            playerHud2.SetSp(playerClass2.currentSp);
                            dialogueText.text = enemyClass2.nameP + " atakuje..";
                            yield return new WaitForSeconds(1f);
                            EnemyTeam[1].transform.position = enemyTwoPos.position;
                            enemyClass2.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            if (isDeadP2)
                            {
                                state = BattleState.lost;
                                EndBattle();
                            }
                            else
                            {
                                state = BattleState.playerTurn;
                                PlayerTurn();
                            }
                            break;
                    }
                    break;
            }
        }
        public void EndBattle()
        {
            if(state == BattleState.won)
            {
                dialogueText.text = "Zwyciêstwo!";
                foreach (Transform child in GameObject.FindGameObjectWithTag("Canvas").transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else if(state == BattleState.lost)
            {
                dialogueText.text = "Przegra³eœ.";
                foreach (Transform child in GameObject.FindGameObjectWithTag("Canvas").transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }
}
