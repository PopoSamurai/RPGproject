using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public enum BattleState
{
    start,
    playerTurn,
    enemyTurn,
    won,
    lost,
}
public class Fight : MonoBehaviour
{
    BattleState state;
    public GameObject[] team, enemys;
    public Text dialogueText;
    public int turnP = 0;
    public int turnE = 0;
    public CharacterClass playerClass;
    public CharacterClass playerClass2;
    public CharacterClass enemyClass;
    public CharacterClass enemyClass2;
    //HUD
    public HudPanel playerHud;
    public HudPanel playerHud2;
    public HudPanel enemyHud;
    public HudPanel enemyHud2;
    //
    bool win = false;
    bool ded = false;
    //
    public GameObject[] EnemyTeam;
    //pos players
    public Transform playerFirstPos;
    public Transform playerTwoPos;
    //pos enemy
    public Transform enemyFirstPos;
    public Transform enemyTwoPos;

    public Vector3 offset;
    public Vector3 offsetE;
    public GameObject[] effect;
    //private void Update()
    //{
    //    foreach (GameObject element in EnemyTeam)
    //    {
    //        if (EnemyTeam[0] == null && EnemyTeam[1] == null && EnemyTeam[2] == null && EnemyTeam[3] == null)
    //        {
    //            dialogueText.text = "Zwyci stwo!";
    //            win = true;
    //            StartCoroutine(GoBack());
    //        }
    //    }

    //    foreach (GameObject element in team.allCharacters)
    //    {
    //        if (team.allCharacters[0] == null && team.allCharacters[1] == null && team.allCharacters[2] == null && team.allCharacters[3] == null)
    //        {
    //            dialogueText.text = "Pora ka...";
    //            ded = true;
    //            StartCoroutine(DedScreen());
    //        }
    //    }
    //}
    public void battleOn()
    {
        state = BattleState.start;
        StartCoroutine(StartBattle());
    }
    public IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(PlayerTurn());
    }
    public IEnumerator PlayerTurn()
    {
        dialogueText.text = "Twoja tura: ";
        turnP += 1;
        yield return new WaitForSeconds(2f);
        //getHitEnemy();
    }
    public IEnumerator EnemyTurn()
    {
        Debug.Log("tura wroga");
        yield return new WaitForSeconds(2f);
        //getHitPlayer();
    }
    public IEnumerator Win()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("wygrana");
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator Lost()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("przegrana");
        //okno konca
        //gamePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
    //heal
    public void HealButton()
    {
        if (state != BattleState.playerTurn)
        {
            return;
        }
        switch (turnP)
        {
            case 1:
                if (playerClass.currentSp >= 10 && playerClass.currentHp < playerClass.health)
                {
                    StartCoroutine(PlayerHeal());
                }
                else
                {
                    dialogueText.text = "Nie jeste  ranny";
                }
                break;
            case 2:
                if (playerClass2.currentSp >= 10 && playerClass2.currentHp < playerClass2.health)
                {
                    StartCoroutine(PlayerHeal());
                }
                else
                {
                    dialogueText.text = "Nie jeste  ranny";
                }
                break;
        }
    }
    IEnumerator PlayerHeal()
    {
        switch (turnP)
        {
            case 1:
                playerClass.Heal(5, 10);
                playerHud.SetHp(playerClass.currentHp);
                playerHud.SetSp(playerClass.currentSp);
                dialogueText.text = "Czujesz przyp yw zdrowia!";
                //disableColors();
                yield return new WaitForSeconds(2f);
                break;
            case 2:
                playerClass2.Heal(5, 10);
                playerHud2.SetHp(playerClass2.currentHp);
                playerHud2.SetSp(playerClass2.currentSp);
                dialogueText.text = "Czujesz przyp yw zdrowia!";
                //disableColors();
                yield return new WaitForSeconds(2f);
                break;
        }
        state = BattleState.enemyTurn;
        StartCoroutine(EnemyTurn());
    }
    //public IEnumerator PlayerAttack()
    //{
    //    switch (turnP)
    //    {
    //        case 1:
    //            playerClass.AttackCo();
    //            switch (turnE)
    //            {
    //                case 1:
    //                    bool isDeadE1 = enemyClass.TakeDamage(playerClass.damage);
    //                    if (playerClass.classChar != CharClass.mage)
    //                    {
    //                        playerClass.GetComponent<SpriteRenderer>().sortingOrder = 3;
    //                        playerClass.transform.position = enemyFirstPos.transform.position + offset;
    //                        Instantiate(effect[2], enemyFirstPos);
    //                    }
    //                    else
    //                    {
    //                        Instantiate(effect[0], enemyFirstPos);
    //                    }
    //                    enemyHud.SetHp(enemyClass.currentHp);
    //                    enemyHud.SetSp(enemyClass.currentSp);
    //                    if (isDeadE1)
    //                    {
    //                        dialogueText.text = "Wr g " + enemyClass.nameP + " nie ¿yje";
    //                        enemyHud.gameObject.SetActive(false);
    //                        //destroy prefab enemy
    //                        //EnemyTeam[0] = null;
    //                        //Destroy(enemyClass.gameObject);
    //                        //e1 = 1;
    //                    }
    //                    else
    //                    {
    //                        dialogueText.text = "Atak si  uda .";
    //                    }
    //                    yield return new WaitForSeconds(2f);
    //                    //team.allCharacters[0].transform.position = playerFirstPos.position;
    //                    playerClass.GetComponent<SpriteRenderer>().sortingOrder = 2;
    //                    if (win == true)
    //                    {
    //                        state = BattleState.won;
    //                        //EndBattle();
    //                    }
    //                    else
    //                    {
    //                        state = BattleState.enemyTurn;
    //                        StartCoroutine(EnemyTurn());
    //                    }
    //                break;
    //            }
    //    }
    //}
}