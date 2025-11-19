using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
{
    public static DeckController Instance;
    public Text enegyText;
    public int maxEnergy = 3; //Max energia
    public int currentEnergy;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private List<GameObject> deckPrefabs;
    private RectTransform rect;
    private HorizontalLayoutGroup layoutGroup;

    private Queue<GameObject> deck = new Queue<GameObject>(); //talia
    public List<Card> cards = new List<Card>(); //karty w tali
    [SerializeField] private int maxHandSize = 7;
    [SerializeField] private int startingHandSize = 5;
    public bool end = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        currentEnergy = maxEnergy;
        enegyText.text = currentEnergy + "/" + maxEnergy;
        rect = GetComponent<RectTransform>();
        layoutGroup = GetComponent<HorizontalLayoutGroup>();

        InitializeDeck();
        DrawStartingHand();
    }
    private void InitializeDeck()
    {
        List<GameObject> shuffledDeck = new List<GameObject>(deckPrefabs);
        shuffledDeck = shuffledDeck.OrderBy(x => Random.value).ToList(); // Tasowanie kart

        foreach (GameObject cardPrefab in shuffledDeck)
        {
            deck.Enqueue(cardPrefab);
        }
    }
    private void DrawStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCard();
        }
    }
    private void DrawCard()
    {
        if (deck.Count > 0 && cards.Count < maxHandSize)
        {
            GameObject cardPrefab = deck.Dequeue();

            GameObject newSlot = Instantiate(slotPrefab, transform);

            foreach (Transform child in newSlot.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject newCardObj = Instantiate(cardPrefab, newSlot.transform, false);
            Card newCard = newCardObj.GetComponent<Card>();

            newCard.gameObject.SetActive(true);
            cards.Add(newCard);
        }
    }
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
                    cards.Remove(card);
                    Destroy(card.gameObject);
                    if (cardParent != null) Destroy(cardParent.gameObject);
                });
            }
        }

        while (cards.Count < maxHandSize && deck.Count > 0)
        {
            DrawCard();
        }
    }
    //ENERGY
    public bool CanUseCard(int cost)
    {
        UpdateEnergyText();
        return currentEnergy >= cost;
    }

    public void UseEnergy(int cost)
    {
        if (currentEnergy >= cost)
        {
            currentEnergy -= cost;
            UpdateEnergyText();
        }
    }
    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
        UpdateEnergyText();
    }
    public void UpdateEnergyText()
    {
        enegyText.text = $"{currentEnergy}/{maxEnergy}";
    }
}