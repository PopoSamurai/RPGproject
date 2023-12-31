using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Slider hpBar;
    public Slider hpBarE;
    public Text hpInt;
    public Text hpIntE;
    public int currentHealth;
    public int currentHealthE;
    public GameObject gamePanel;
    public Image playerSprite, enemySprite;
    public Text InfoTxt;
    //
    public Material defaultMat, flash;
    //
    private Inventory inventory;
    public Item[] itemy;
    public int randEq;
    void Start()
    {
        InfoTxt.text = "";
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
        inventory = GameObject.FindGameObjectWithTag("EQ").GetComponent<Inventory>();
    }
    private void Update()
    {
        if(enemy == null)
        {
            //dsdsadadsad
        }
        else
        {
            //enemy
            hpBarE.maxValue = enemy.Vitality;
            hpIntE.text = currentHealthE + "/" + enemy.Vitality;
            hpBarE.value = currentHealthE;
        }
        //player
        hpBar.maxValue = VIT;
        hpInt.text = currentHealth + "/" + VIT;
        hpBar.value = currentHealth;

        if(Input.GetKeyDown(KeyCode.A))
        {
            randEq = Random.Range(0, itemy.Count());

            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (inventory.isFull[inventory.slots.Length - 1] == true)
                {
                    for (int j = 0; j < inventory.slots.Length - 1; j++)
                    {
                        if (inventory.slots[inventory.slots.Length - 1])
                        {
                            inventory.slots[inventory.slots.Length - 1].GetComponent<Image>().sprite = itemy[randEq].Icon;
                        }
                        inventory.slots[j].GetComponent<Image>().sprite = inventory.slots[j + 1].GetComponent<Image>().sprite;
                        //inventory.slots[j] = inventory.slots[j + 1];
                        Debug.Log("auuu");
                    }
                    i--;
                }
                if (inventory.isFull[i] == false)
                {
                    //Debug.Log("item dodany");
                    inventory.slots[i].GetComponent<Image>().color = Color.white;
                    inventory.slots[i].GetComponent<Image>().sprite = itemy[randEq].Icon;
                    inventory.isFull[i] = true;
                    break;
                }
            }
        }
    }
    public void getHitPlayer()
    {
        InfoTxt.text = "Wr�g atakuje";
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
        InfoTxt.text = "Gracz atakuje";
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
        InfoTxt.text = "twoja tura";
        yield return new WaitForSeconds(2f);
        InfoTxt.text = "Atakujesz";
        yield return new WaitForSeconds(1f);
        getHitEnemy();
    }
    public IEnumerator EnemyTurn()
    {
        InfoTxt.text = "tura wroga";
        yield return new WaitForSeconds(2f);
        InfoTxt.text = "Otrzymujesz obra�enia";
        yield return new WaitForSeconds(1f);
        getHitPlayer();
    }
    public IEnumerator Win()
    {
        yield return new WaitForSeconds(2f);
        InfoTxt.text = "wygrana";
        //
        enemy = null;
        generator.battleTurn();
        gamePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator Lost()
    {
        yield return new WaitForSeconds(2f);
        InfoTxt.text = "przegrana";
        enemy = null;
        //okno konca
        gamePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
}
