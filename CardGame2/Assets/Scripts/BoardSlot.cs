using UnityEngine;
using UnityEngine.EventSystems;
public class BoardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool occupied;
    public void OnDrop(PointerEventData eventData)
    {
        var cardView = eventData.pointerDrag?.GetComponent<CardView>();
        if (cardView == null) return;

        if (cardView.data.type != CardType.Character)
        {
            cardView.WasPlayedOnBoard = false;
            return;
        }
        if (occupied)
        {
            cardView.ReturnToSlot();
            return;
        }

        bool firstTime = cardView.CurrentSlot == null;
        bool success = CardExecutor.Instance.TryPlayUnitCard(cardView, this, firstTime);

        if (!success)
            cardView.ReturnToSlot();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 
    }
}