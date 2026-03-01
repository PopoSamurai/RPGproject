using UnityEngine;
using UnityEngine.EventSystems;
public enum SlotOwner
{
    Player,
    Enemy
}
public class BoardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool occupied;
    public SlotOwner owner;
    public void OnDrop(PointerEventData eventData)
    {
        var cardView = eventData.pointerDrag?.GetComponent<CardView>();
        if (cardView == null) return;

        if (cardView.WasPlayedOnBoard)
            return;

        if (cardView.data.type != CardType.Character)
        {
            cardView.WasPlayedOnBoard = false;
            return;
        }

        if (occupied)
        {
            cardView.ReturnToHand();
            return;
        }

        bool firstTime = cardView.CurrentSlot == null;
        bool success = CardExecutor.Instance.TryPlayUnitCard(cardView, this, firstTime, false);

        if (!success)
            cardView.ReturnToHand();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (owner == SlotOwner.Enemy) return;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (owner == SlotOwner.Enemy) return;
    }
}