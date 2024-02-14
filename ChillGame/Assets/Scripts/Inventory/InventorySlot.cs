using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool eqSlot;
    public bool shopSlot;
    public bool rushBin = false;
    public SlotType type;
    GameObject gamem;
    private void Start()
    {
        gamem = GameObject.FindGameObjectWithTag("gamem");
    }
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
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                    inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
                }
            }
            else if (transform.GetChild(0).GetComponent<InventoryItem>().item.isStack == true && transform.GetChild(0).GetComponent<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
                inventoryItem.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
            else
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
                inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
            }
        }
        //sell
        else if (shopSlot == true)
        {
            InventoryItem inventoryItem3 = eventData.pointerDrag.GetComponent<InventoryItem>();

            if (transform.childCount == 0 && inventoryItem3.parentAfterDrag.transform.GetComponent<InventorySlot>().shopSlot == false)
            {
                inventoryItem3.parentAfterDrag = transform;
                gamem.GetComponent<InventoryManager>().money += inventoryItem3.costItem;
                Debug.Log("buy");
            }
            //not swap
            else if (transform.childCount == 1 && transform.GetChild(0).GetComponent<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
            {
                Debug.Log("Its not free slot");
            }
            //connect items
            else if (transform.GetChild(0).GetComponent<InventoryItem>().item.isStack == true && transform.GetChild(0).GetComponent<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                InventoryItem inventoryItem4 = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                inventoryItem3.parentAfterDrag = transform;
                if(inventoryItem4.parentAfterDrag.transform.GetComponent<InventorySlot>().shopSlot == false)
                {
                    gamem.GetComponent<InventoryManager>().money += inventoryItem4.costItem;
                }
                inventoryItem3.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem3.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
            else
            {
                inventoryItem3.parentAfterDrag = transform;
            }
        }
        //rushbin
        else if(rushBin == true)
        {
            Destroy(eventData.pointerDrag);
        }
        //buy
        else if(eventData.pointerDrag.GetComponent<InventoryItem>().parentAfterDrag.transform.GetComponent<InventorySlot>().shopSlot == true)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

            if (gamem.GetComponent<InventoryManager>().money >= inventoryItem.costItem)
            {
                //buy on free slot
                if(transform.childCount == 0)
                {
                    inventoryItem.parentAfterDrag = transform;
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.costItem;
                }
                //swap items
                else if (transform.childCount == 1 && transform.GetChild(0).GetComponent<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
                {
                    Debug.Log("Its not free slot");
                }
                //connect items
                else if (transform.GetChild(0).GetComponent<InventoryItem>().item.isStack == true && transform.GetChild(0).GetComponent<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
                {
                    InventoryItem inventoryItem3 = eventData.pointerDrag.GetComponent<InventoryItem>();
                    InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                    inventoryItem3.parentAfterDrag = transform;
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.costItem;
                    inventoryItem3.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                    inventoryItem3.GetComponent<InventoryItem>().RefreshCount();
                    Destroy(inventoryItem2.gameObject);
                }
            }
            else
            {
                Debug.Log("You don't have money farmer");
            }
        }
        else
        {
            //change item position
            if (transform.childCount == 0)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
            }
            //swap items
            else if (transform.childCount == 1 && transform.GetChild(0).GetComponent<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item) // swap items
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
                inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
            }
            //connect items
            else if (transform.GetChild(0).GetComponent<InventoryItem>().item.isStack == true && transform.GetChild(0).GetComponent<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetChild(0).GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
                inventoryItem.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
        }
    }
}