using System.Collections;
using UnityEngine.UI;
using UnityEngine;
public enum Turn
{
    Player,
    Enemy
}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public Turn currentTurn;
    public float enemyPlayDelay = 1f;
    public int turnNumber = 0;
    public GameObject turnText;
    public bool IsPlayerTurn => currentTurn == Turn.Player;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        StartPlayerTurn();
    }
    public void UpdateTurn()
    {
        turnNumber++;
        turnText.GetComponent<Text>().text = "Turn " + turnNumber.ToString();
    }
    public void StartPlayerTurn()
    {
        UpdateTurn();
        currentTurn = Turn.Player;
        FindObjectOfType<HandManager>().FillHandToMax();
    }
    public void EndPlayerTurn()
    {
        UpdateTurn();
        StartCoroutine(EnemyTurnRoutine());
    }
    IEnumerator EnemyTurnRoutine()
    {
        currentTurn = Turn.Enemy;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(BattleSystem.Instance.TurnRoutine());

        EndEnemyTurn();
    }
    void EndEnemyTurn()
    {
        StartPlayerTurn();
    }
}