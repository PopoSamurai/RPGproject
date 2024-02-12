using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool eqSlot;
    public SlotType type;
    public enum SlotType
    {
        None,
        Sword,
        water,
        fishRod,
        seed,
        crop
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eqSlot == true)
        {
            InventoryItem item = eventData.pointerDrag.GetComponent<InventoryItem>();

            if (transform.childCount == 0)
            {
                if ((int)type == (int)item.item.itemtype)
                {
                    InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                    inventoryItem.parentAfterDrag = transform;
                }
            }
            else if (transform.childCount == 1)
            {
                if ((int)type == (int)item.item.itemtype)
                {
                    InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                    InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                    inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
                }
            }
        }
        else
        {
            if (transform.childCount == 0)
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
}