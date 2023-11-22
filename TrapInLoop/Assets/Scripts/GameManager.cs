using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public enum BattleState
{
    start,
    playerTurn,
    enemyTurn,
    won,
    lost,
}
public class GameManager : MonoBehaviour
{
    public Character player;
    public Enemy enemy;
    int STR, LCK, SPD, VIT, DEF, INT, WIS, CHA;
    [SerializeField]Text[] staty;
    public MapGenerator generator;
    public Scrollbar hpBar;
    public Scrollbar hpBarE;
    public Text hpInt;
    public Text hpIntE;
    public int currentHealth;
    public int currentHealthE;
    public GameObject gamePanel;
    public Image playerSprite, enemySprite;
    //
    public Material defaultMat, flash;
    void Start()
    {        
        //pobiera od gracza
        STR = player.Strength;
        LCK = player.Luck;
        SPD = player.Speed;
        VIT = player.Vitality;
        DEF = player.Defence;
        INT = player.Intelligence;
        WIS = player.Wisdom;
        CHA = player.Charisma;
        //do textboxa
        staty[0].text = "" + STR;
        staty[1].text = "" + LCK;
        staty[2].text = "" + SPD;
        staty[3].text = "" + VIT;
        staty[4].text = "" + DEF;
        staty[5].text = "" + INT;
        staty[6].text = "" + WIS;
        staty[7].text = "" + CHA;
        playerSprite.material = defaultMat;
        enemySprite.material = defaultMat;
        gamePanel.SetActive(false);
        currentHealth = VIT;
    }
    private void Update()
    {
        if(enemy == null)
        {

        }
        else
        {
            //enemy
            hpBarE.size = currentHealthE;
            hpIntE.text = currentHealthE + "/" +enemy.Vitality;
        }
        //player
        hpBar.size = currentHealth;
        hpInt.text = currentHealth + "/" + VIT;
    }
    public void getHitPlayer()
    {
        Debug.Log("Wróg atakuje");
        StartCoroutine(lightPlayer());
        currentHealth -= enemy.Strength;
        if (currentHealth > 0)
        {
            StartCoroutine(PlayerTurn());
        }
        //end
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Lost());
        }
    }
    IEnumerator lightPlayer()
    {
        playerSprite.material = flash;
        yield return new WaitForSeconds(1f);
        playerSprite.material = defaultMat;
    }
    IEnumerator lightEnemy()
    {
        enemySprite.material = flash;
        yield return new WaitForSeconds(1f);
        enemySprite.material = defaultMat;
    }
    public void getHitEnemy()
    {
        Debug.Log("Gracz atakuje");
        StartCoroutine(lightEnemy());
        currentHealthE -= STR;
        if (currentHealthE > 0)
        {
            StartCoroutine(EnemyTurn());
        }
        //end
        if (currentHealthE <= 0)
        {
            currentHealthE = 0;
            StartCoroutine(Win());
        }
    }
    public void battleOn()
    {
        StartCoroutine(StartBattle());
    }
    public IEnumerator StartBattle()
    {
        currentHealthE = enemy.Vitality;
        gamePanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        StartCoroutine(PlayerTurn());
    }
    public IEnumerator PlayerTurn()
    {
        Debug.Log("twoja tura");
        yield return new WaitForSeconds(2f);
        Debug.Log("Atakujesz");
        yield return new WaitForSeconds(1f);
        getHitEnemy();
    }
    public IEnumerator EnemyTurn()
    {
        Debug.Log("tura wroga");
        yield return new WaitForSeconds(2f);
        Debug.Log("Otrzymujesz obra¿enia");
        yield return new WaitForSeconds(1f);
        getHitPlayer();
    }
    public IEnumerator Win()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("wygrana");
        enemy = null;
        generator.battleTurn();
        gamePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator Lost()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("przegrana");
        enemy = null;
        //okno konca
        gamePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
}
