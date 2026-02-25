using UnityEngine;
using System.Collections.Generic;
public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handParent;
    public List<CardData> deck = new List<CardData>();
    public List<CardView> hand = new List<CardView>();
    public int maxHandSize = 6;
    public void DrawCard()
    {
        if (deck.Count == 0) return;
        if (hand.Count >= maxHandSize) return;

        CardData data = deck[Random.Range(0, deck.Count)];

        var cardGO = Instantiate(cardPrefab, handParent);
        var view = cardGO.GetComponent<CardView>();
        view.Init(data);

        hand.Add(view);
        FindObjectOfType<ArcLayoutGroup>().UpdateLayout();
    }
    void Start()
    {
        FillHandToMax();
        FindObjectOfType<ArcLayoutGroup>().UpdateLayout();
    }
    public void FillHandToMax()
    {
        while (hand.Count < maxHandSize)
        {
            DrawCard();
            FindObjectOfType<ArcLayoutGroup>().UpdateLayout();
        }
    }
    public void RemoveCard(CardView card)
    {
        hand.Remove(card);
        Destroy(card.gameObject);
        FindObjectOfType<ArcLayoutGroup>().UpdateLayout();
    }
}