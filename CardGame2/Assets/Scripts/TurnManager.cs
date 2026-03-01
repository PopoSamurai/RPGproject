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
        StartEnemyTurn();
    }
    void StartEnemyTurn()
    {
        currentTurn = Turn.Enemy;
        Debug.Log("Enemy turn started");

        StartCoroutine(EnemyPlayRoutine());
    }
    void EndEnemyTurn()
    {
        Debug.Log("Enemy turn ended");
        StartPlayerTurn();
    }
    IEnumerator EnemyPlayRoutine()
    {
        BoardSlot[] enemySlots = FindObjectsOfType<BoardSlot>()
            .Where(s => s.owner == SlotOwner.Enemy && !s.occupied)
            .ToArray();

        foreach (var slot in enemySlots)
        {
            var card = EnemyDeck.Instance.GetRandomUnitCard();
            if (card != null)
            {
                var cardGO = Instantiate(EnemyDeck.Instance.cardPrefab);
                var cardView = cardGO.GetComponent<CardView>();
                cardView.Init(card);

                CardExecutor.Instance.TryPlayUnitCard(cardView, slot, true, true);
                yield return new WaitForSeconds(enemyPlayDelay);
            }
        }
        EndEnemyTurn();
    }
}