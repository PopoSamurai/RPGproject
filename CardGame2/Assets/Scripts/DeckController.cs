using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [SerializeField] private Card selectedCard;
    [SerializeField] private GameObject slotPrefab;
    private RectTransform rect;

    public List<Card> cards;
    [SerializeField] private int cardsInt = 7;
    [SerializeField] private bool tweenCardReturn = true;
    private void Start()
    {
        for (int i = 0; i < cardsInt; i++)
        {
            Instantiate(slotPrefab, transform);
        }

        rect = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;

        foreach (Card card in cards)
        {
            card.pointEnter.AddListener(CardPointerEnter);
            card.pointExit.AddListener(CardPointerExit);
            card.beginDrag.AddListener(BeginDrag);
            card.endDrag.AddListener(EndDrag);
            card.name = cardCount.ToString();
            cardCount++;
        }

    }
    private void BeginDrag(Card card)
    {
        selectedCard = card;
    }
    void EndDrag(Card card)
    {
        if (selectedCard == null)
            return;

        selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;

    }
    void CardPointerEnter(Card card)
    {
        
    }

    void CardPointerExit(Card card)
    {
        
    }
}
