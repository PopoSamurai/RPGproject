using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
{
    [SerializeField] private Card selectedCard;
    [SerializeField] private GameObject slotPrefab;
    private RectTransform rect;
    private HorizontalLayoutGroup layoutGroup;

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
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
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

        selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;
    }
    void CardPointerEnter(Card card) { }
    void CardPointerExit(Card card) { }
    public void EndTurn()
    {
        List<Card> cardsToRemove = cards.Where(card => card.selected).ToList();

        foreach (Card card in cardsToRemove)
        {
            if (card != null)
            {
                Transform cardParent = card.transform.parent;

                card.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
                {
                    Destroy(card.gameObject);
                    if (cardParent != null) Destroy(cardParent.gameObject);
                    cards.Remove(card);
                });
            }
        }
    }
}