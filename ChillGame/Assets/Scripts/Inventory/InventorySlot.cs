using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
        else if (transform.childCount == 1)
        {
            //swap
            InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
            inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
            inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
        }
    }
}