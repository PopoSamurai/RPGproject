using System.Collections;
using System.Linq;
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
    public bool IsPlayerTurn => currentTurn == Turn.Player;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        StartPlayerTurn();
    }
    public void StartPlayerTurn()
    {
        currentTurn = Turn.Player;
        Debug.Log("Player turn started");
        FindObjectOfType<HandManager>().FillHandToMax();
    }
    public void EndPlayerTurn()
    {
        Debug.Log("Player turn ended");
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
        Debug.Log("Enemy turn ended");
        StartPlayerTurn();
    }
}