using UnityEngine;
public class CardExecutor : MonoBehaviour
{
    public static CardExecutor Instance;
    public PlayerEnergySystem energySystem;
    void Awake()
    {
        Instance = this;
    }
    public bool TryPlayUnitCard(CardView cardView, BoardSlot slot, bool isFirstTime, bool isAI)
    {
        if (!isAI && slot.owner == SlotOwner.Enemy)
        {
            Debug.Log("Player tried to play on enemy slot");
            return false;
        }
        var card = cardView.data;

        if (card.type != CardType.Character)
        {
            Debug.Log("FAIL: not a unit card");
            return false;
        }
        if (!energySystem.HasEnoughEnergy(card.energyCost))
        {
            Debug.Log("FAIL: not enough energy");
            return false;
        }
        if (slot.occupied)
        {
            Debug.Log("FAIL: slot occupied");
            return false;
        }
        if (isFirstTime)
        {
            if (!isAI)
                energySystem.UseEnergy(card.energyCost);

            var unit = cardView.gameObject.GetComponent<Unit>();

            if (unit == null)
            {
                unit = cardView.gameObject.AddComponent<Unit>();
                unit.Init(card);
                cardView.AttachedUnit = unit;
            }
        }
        PlaceCardOnSlot(cardView, slot);

        if (cardView.CurrentSlot != null && cardView.CurrentSlot != slot)
            cardView.CurrentSlot.occupied = false;

        cardView.CurrentSlot = slot;
        cardView.UpdateStatsUI();

        var boardTarget = cardView.gameObject.GetComponent<BoardTarget>();
        if (boardTarget == null)
            boardTarget = cardView.gameObject.AddComponent<BoardTarget>();

        GameObject snap = new GameObject("SnapPoint");
        snap.transform.SetParent(cardView.transform, false);
        RectTransform snapRT = snap.AddComponent<RectTransform>();
        snapRT.anchorMin = new Vector2(0.5f, 1f);
        snapRT.anchorMax = new Vector2(0.5f, 1f);
        snapRT.pivot = new Vector2(0.5f, 0.5f);
        snapRT.anchoredPosition = new Vector2(0, 20f);
        boardTarget.snapPoint = snapRT;

        PlaceCardOnSlot(cardView, slot);
        cardView.UpdateStatsUI();
        return true;
    }
    void PlaceCardOnSlot(CardView cardView, BoardSlot slot)
    {
        var handManager = FindObjectOfType<HandManager>();
        handManager.hand.Remove(cardView);

        RectTransform rt = cardView.GetComponent<RectTransform>();
        rt.SetParent(slot.transform, false);
        rt.localPosition = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        cardView.IsDragging = false;
        cardView.WasPlayedOnBoard = true;

        var cg = cardView.GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = true;

        var hover = cardView.GetComponent<CardHoverUI>();
        if (hover) hover.enabled = false;

        slot.occupied = true;

        FindObjectOfType<ArcLayoutGroup>().UpdateLayout(true);
    }
    public void UseCard(CardData card, GameObject target, bool isAI)
    {
        if (!isAI)
        {
            if (!energySystem.HasEnoughEnergy(card.energyCost))
            {
                Debug.Log("Not enough energy!");
                return;
            }

            energySystem.UseEnergy(card.energyCost);
        }
        switch (card.type)
        {
            case CardType.Character:
                SpawnCharacter(card);
                break;

            case CardType.Attack:
                DealDamage(target, card.damageAmount);
                break;

            case CardType.Heal:
                HealTarget(target, card.healAmount);
                break;

            case CardType.Energy:
                energySystem.AddEnergy(card.energyAmount);
                break;
        }
    }
    public void OnCardClicked(CardView card)
    {
        Debug.Log("Clicked card: " + card.data.cardName);
        if (card.data.type == CardType.Energy)
        {
            UseCard(card.data, null, false);
        }
        else
        {
            TargetingSystem.Instance.StartTargeting(card.data);
        }
    }
    void SpawnCharacter(CardData card)
    {
        Debug.Log("Spawn character: " + card.cardName);
    }
    void DealDamage(GameObject target, int dmg)
    {
        Debug.Log("Deal " + dmg + " dmg");
        target.GetComponent<Unit>().TakeDamage(dmg);
    }
    void HealTarget(GameObject target, int heal)
    {
        Debug.Log("Heal " + heal);
        target.GetComponent<Unit>().Heal(heal);
    }
}