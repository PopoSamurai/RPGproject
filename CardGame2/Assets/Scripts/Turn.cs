using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turn : MonoBehaviour
{
    public static Turn Instance;
    public DeckController Controller;
    [SerializeField] private Button endturn;
    [SerializeField] private Slider playerHp;
    [SerializeField] private Slider enemyHp;

    public int playerMaxHp;
    public int playercurrentHp;
    public int enemyMaxHp;
    public int enemycurrentHp;

    public int enemyDamage;
    public int playerDamage;

    [SerializeField] private Text playerHpText;
    [SerializeField] private Text enemyHpText;
    public int turn = 0;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        playercurrentHp = playerMaxHp;
        playerHp.maxValue = playerMaxHp;
        playerHp.value = playercurrentHp;

        enemycurrentHp = enemyMaxHp;
        enemyHp.maxValue = enemyMaxHp;
        enemyHp.value = enemycurrentHp;

        UpdateHpUI();
        endturn.onClick.AddListener(EndTurn);
    }

    void UpdateHpUI()
    {
        playerHp.value = playercurrentHp;
        playerHpText.text = playercurrentHp + "/" + playerMaxHp;

        enemyHp.value = enemycurrentHp;
        enemyHpText.text = enemycurrentHp + "/" + enemyMaxHp;
    }

    public void EndTurn()
    {
        if (turn == 0)
        {
            Debug.Log("Gracz koñczy turê!");
            AttackEnemy();
            playerDamage = 0;
            Controller.EndTurn();
            turn = 1;
            endturn.interactable = false;
            StartCoroutine(EnemyTurn());
        }
    }
    public void AddDamage(int dmg)
    {
        playerDamage += dmg;
    }

    public void RemoveDamage(int dmg)
    {
        playerDamage -= dmg;
    }
    IEnumerator EnemyTurn()
    {
        Debug.Log("Tura wroga zaczyna siê...");
        yield return new WaitForSeconds(2f);

        AttackPlayer();
        UpdateHpUI();

        yield return new WaitForSeconds(1f);

        turn = 0;
        Debug.Log("Tura gracza");

        Controller.ResetEnergy();
        endturn.interactable = true;
    }

    public void AttackEnemy()
    {
        enemycurrentHp -= playerDamage;
        UpdateHpUI();
        CheckGameOver();
    }

    public void AttackPlayer()
    {
        playercurrentHp -= enemyDamage;
        UpdateHpUI();
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (playercurrentHp <= 0)
        {
            Debug.Log("Gracz przegra³!");
            endturn.interactable = false;
        }
        else if (enemycurrentHp <= 0)
        {
            Debug.Log("Wróg przegra³!");
            endturn.interactable = false;
        }
    }
}