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

    public BoardLane line;
    public int indexInLine; // 0 = front, 2 = back

    void Start()
    {
        if (line == null)
            Debug.LogError($"{name} has NO LINE!");
    }
    public void OnDrop(PointerEventData eventData)
    {
        var cardView = eventData.pointerDrag?.GetComponent<CardView>();

        if (cardView == null) return;
        if (cardView.WasPlayedOnBoard) return;

        if (cardView.IsDragging)
            return;
        if (cardView.data.type != CardType.Character)
        {
            cardView.WasPlayedOnBoard = false;
            return;
        }
        BoardLane line = GetComponentInParent<BoardLane>();
        if (line == null)
        {
            Debug.LogError("[OnDrop] Slot nie nale¿y do ¿adnej linii!");
            cardView.ReturnToHand();
            return;
        }
        if (occupied)
        {
            Debug.Log("Slot occupied!");
            cardView.ReturnToHand();
            return;
        }
        bool success = CardExecutor.Instance.TryPlayUnitCard(
            cardView,
            this,
            true,
            false
        ); 

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