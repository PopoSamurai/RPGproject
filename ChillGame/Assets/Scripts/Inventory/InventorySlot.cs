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
            else if (transform.childCount == 1 && transform.GetChild(0).GetComponent<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
            {
                if ((int)type == (int)item.item.itemtype)
                {
                    InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                    InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                    Debug.Log("swap items");
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                    inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
                }
            }
            else if (transform.GetChild(0).GetComponent<InventoryItem>().item.isStack == true && transform.GetChild(0).GetComponent<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                Debug.Log("connect items");
                inventoryItem.parentAfterDrag = transform;
                inventoryItem.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
            else
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                Debug.Log("swap items");
                inventoryItem.parentAfterDrag = transform;
                inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
            }
        }
        else
        {
            if (transform.childCount == 0)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
            }
            else if (transform.childCount == 1 && transform.GetChild(0).GetComponent<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                Debug.Log("swap items");
                inventoryItem.parentAfterDrag = transform;
                inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
            }
            else if(transform.GetChild(0).GetComponent<InventoryItem>().item.isStack == true && transform.GetChild(0).GetComponent<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                Debug.Log("connect items");
                inventoryItem.parentAfterDrag = transform;
                inventoryItem.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
            else
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                Debug.Log("swap items");
                inventoryItem.parentAfterDrag = transform;
                inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
            }
        }
    }
}