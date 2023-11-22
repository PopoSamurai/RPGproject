using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
public enum MapState
{
    start,
    roomCheck,
    battleTurn,
}
public class MapGenerator : MonoBehaviour
{
    public Image[] rooms;
    public Color start;
    public int number = 0;
    public bool fix = false;
    public Text loop;
    public int loopNum = 1;
    public MapState state;
    public float speed = 1f;
    GameObject gameM;
    //enemy
    public int randInt = 0;
    void Start()
    {
        gameM = GameObject.FindGameObjectWithTag("GameM");
        loop.text = "Loop: " + loopNum;
    }
    private void Update()
    {
        state = MapState.start;
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
                    state = MapState.roomCheck;
                    roomCheck();
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
        state = MapState.start;
        GeneratorOn();
    }
    public void roomCheck()
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
            state = MapState.battleTurn;
            StartCoroutine(battleTurn());
        }
        if (rooms[number].GetComponent<Biome>().biom == BiomeName.dungeon)
        {
            Debug.Log("Walka");
            state = MapState.battleTurn;
            StartCoroutine(battleTurn());
        }
    }
    public IEnumerator battleTurn()
    {
        yield return new WaitForSeconds(speed);
        gameM.GetComponent<GameManager>().battleOn();
        rooms[number].GetComponent<Biome>().ResetCol();
        //number += 1;
        //fix = false;
        //state = MapState.start;
        //GeneratorOn();
    }
}
