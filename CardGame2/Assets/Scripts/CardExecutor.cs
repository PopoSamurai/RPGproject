using UnityEngine;
public class CardExecutor : MonoBehaviour
{
    public static CardExecutor Instance;
    public PlayerEnergySystem energySystem;
    void Awake()
    {
        Instance = this;
    }
    public bool TryPlayUnitCard(CardView cardView, BoardSlot slot)
    {
        var card = cardView.data;
        Debug.Log("TRY PLAY CARD: " + card.cardName);

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
        energySystem.UseEnergy(card.energyCost);
        SpawnCharacterOnSlot(card, slot);
        PlaceCardOnSlot(cardView, slot);

        Debug.Log("SUCCESS: card played");
        return true;
    }
    void SpawnCharacterOnSlot(CardData card, BoardSlot slot)
    {
        Debug.Log("Spawn unit: " + card.cardName + " on slot " + slot.name);
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

        Debug.Log("CARD PARENT = " + rt.parent.name);
    }
    public void UseCard(CardData card, GameObject target)
    {
        if (!energySystem.HasEnoughEnergy(card.energyCost))
        {
            Debug.Log("Not enough energy!");
            return;
        }
        energySystem.UseEnergy(card.energyCost);

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
            UseCard(card.data, null);
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