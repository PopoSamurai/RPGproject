using UnityEngine;
using UnityEngine.EventSystems;
public class BoardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool occupied;
    public void OnDrop(PointerEventData eventData)
    {
        if (occupied) return;

        var cardView = eventData.pointerDrag?.GetComponent<CardView>();
        if (cardView == null) return;

        if (cardView.data.type != CardType.Character)
        {
            cardView.WasPlayedOnBoard = false;
            return;
        }

        bool success = CardExecutor.Instance.TryPlayUnitCard(cardView, this);

        if (!success)
        {
            cardView.WasPlayedOnBoard = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ENTER SLOT: " + name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXIT SLOT: " + name);
    }
}