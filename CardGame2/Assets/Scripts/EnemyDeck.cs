using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EnemyDeck : MonoBehaviour
{
    public static EnemyDeck Instance;
    public List<CardData> deck = new List<CardData>();
    public GameObject cardPrefab;
    void Awake() => Instance = this;
    public CardData GetRandomUnitCard()
    {
        var unitCards = deck.Where(c => c.type == CardType.Character).ToList();
        if (unitCards.Count == 0) return null;

        int index = Random.Range(0, unitCards.Count);
        return unitCards[index];
    }
}