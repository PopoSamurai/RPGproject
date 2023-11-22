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
    int STR, LCK, SPD, VIT, DEF, INT, WIS, CHA;
    [SerializeField]Text[] staty;
    public MapGenerator generator;
    public Scrollbar hpBar;
    public Text hpInt;
    public int currentHealth;
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
        //hpBar
        currentHealth = STR;
        hpInt.text = STR + "/" + currentHealth;
        hpBar.size = STR;
    }
    public void battleOn()
    {
        StartCoroutine(StartBattle());
    }

    public IEnumerator StartBattle()
    {
        if(generator)
        {

        }
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator PlayerTurn()
    {
        Debug.Log("twoja tura");
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator EnemyTurn()
    {
        Debug.Log("tura wroga");
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator Win()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("wygrana");
        generator.number += 1;
        generator.fix = false;
        generator.state = MapState.start;
        generator.GeneratorOn();
    }
    public IEnumerator Lost()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("przegrana");
    }
}
