using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
public enum BattleState
{
    start,
    playerTurn,
    enemyTurn,
    won,
    lost,
}
public class MapGenerator : MonoBehaviour
{
    public Image[] rooms;
    public Color start;
    public int number = 0;
    public bool fix = false;
    public Text loop;
    public int loopNum = 1;
    public BattleState state;
    public float speed = 1f;
    void Start()
    {
        loop.text = "Loop: " + loopNum;
    }
    private void Update()
    {
        state = BattleState.start;
        GeneratorOn();
    }
    //pause
    public void pauseButt()
    {
        speed = 0f;
    }
    public void resumeButt()
    {
        speed = 1f;
    }
    public void GeneratorOn()
    {
        loop.text = "Loop: " + loopNum;

        foreach (Image item in rooms)
        {
            if (number < rooms.Count())
            {
                if (rooms[number] && fix == false)
                {
                    fix = true;
                    rooms[number].color = start;
                    state = BattleState.playerTurn;
                    PlayerTurn();
                }
            }
            else
            {
                number = 0;
                loopNum += 1;
            }
        }
    }
    public IEnumerator stetupBattle()
    {
        yield return new WaitForSeconds(speed);
        rooms[number].GetComponent<Biome>().ResetCol();
        yield return new WaitForSeconds(speed);
        number += 1;
        fix = false;
        state = BattleState.start;
        GeneratorOn();
    }
    public void PlayerTurn()
    {
        if (rooms[number].GetComponent<Biome>().biom == BiomeName.Start)
        {
            Debug.Log("Spawn");
            StartCoroutine(stetupBattle());
        }
        if (rooms[number].GetComponent<Biome>().biom == BiomeName.Empty)
        {
            Debug.Log("Pusto");
            StartCoroutine(stetupBattle());
        }
        if (rooms[number].GetComponent<Biome>().biom == BiomeName.forest)
        {
            Debug.Log("Walka");
            state = BattleState.enemyTurn;
            StartCoroutine(EnemyTour());
        }
        if (rooms[number].GetComponent<Biome>().biom == BiomeName.dungeon)
        {
            Debug.Log("Walka");
            state = BattleState.enemyTurn;
            StartCoroutine(EnemyTour());
        }
    }
    public IEnumerator EnemyTour()
    {
        yield return new WaitForSeconds(speed);
        rooms[number].GetComponent<Biome>().ResetCol();
        yield return new WaitForSeconds(speed);
        number += 1;
        fix = false;
        state = BattleState.start;
        GeneratorOn();
    }
}
