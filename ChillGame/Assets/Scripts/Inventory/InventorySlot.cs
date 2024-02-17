using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public enum SlotType
{
    None,
    Sword,
    water,
    fishRod,
    seed,
    crop
}
public class InventorySlot : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    public bool eqSlot;
    public bool shopSlot;
    public bool rushBin = false;
    public SlotType type;
    GameObject gamem;
    //
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;
    public void OnPointerDown(PointerEventData eventData)
    {
        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;

            if (transform.childCount != 0)
            {
                for (int i = 0; i < gamem.GetComponent<InventoryManager>().inventorySlots.Length; i++)
                {
                    if (eventData.pointerEnter.GetComponent<InventoryItem>().item.type == gamem.GetComponent<InventoryManager>().inventorySlots[i].GetComponent<InventorySlot>().type && gamem.GetComponent<InventoryManager>().inventorySlots[i].GetComponent<InventorySlot>().transform.childCount == 0)
                    {
                        eventData.pointerEnter.GetComponent<InventoryItem>().transform.parent = gamem.GetComponent<InventoryManager>().inventorySlots[i].GetComponent<InventorySlot>().transform;
                    }
                    else if (eventData.pointerEnter.GetComponent<InventoryItem>().item.type == gamem.GetComponent<InventoryManager>().inventorySlots[i].GetComponent<InventorySlot>().type && gamem.GetComponent<InventoryManager>().inventorySlots[i].GetComponent<InventorySlot>().transform.childCount == 1)
                    {
                        Debug.Log("Slot jest zajêty");
                        InventoryItem inventoryItem2 = gamem.GetComponent<InventoryManager>().inventorySlots[i].GetComponent<InventorySlot>().transform.GetComponentInChildren<InventoryItem>();
                        inventoryItem2.transform.parent = eventData.pointerEnter.GetComponent<InventoryItem>().transform.parent;
                        eventData.pointerEnter.GetComponent<InventoryItem>().transform.parent = gamem.GetComponent<InventoryManager>().inventorySlots[i].GetComponent<InventorySlot>().transform;
                    }
                }
            }
            else
            {
                Debug.Log("Empty");
            }
        }
        else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
    }
    private void Start()
    {
        gamem = GameObject.FindGameObjectWithTag("gamem");
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eqSlot == true)
        {
            InventoryItem item = eventData.pointerDrag.GetComponent<InventoryItem>();

            if (transform.childCount == 0)
            {
                if ((int)type == (int)item.item.type)
                {
                    InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                    inventoryItem.parentAfterDrag = transform;
                }
            }
            else if (transform.childCount == 1 && transform.GetChild(0).GetComponent<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
            {
                if ((int)type == (int)item.item.type)
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
        else if (shopSlot == true && eventData.pointerDrag.GetComponent<InventoryItem>().parentAfterDrag.transform.GetComponent<InventorySlot>().shopSlot == false)
        {
            InventoryItem inventoryItem3 = eventData.pointerDrag.GetComponent<InventoryItem>();

            if (transform.childCount == 0)
            {
                inventoryItem3.parentAfterDrag = transform;
                gamem.GetComponent<InventoryManager>().money += inventoryItem3.costItem;
                Debug.Log("sell");
            }
            //not swap
            else if (transform.childCount == 1 && transform.GetComponentInChildren<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
            {
                Debug.Log("Its not free slot");
            }
            //connect items
            else if (transform.GetComponentInChildren<InventoryItem>().item.isStack == true && transform.GetComponentInChildren<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                InventoryItem inventoryItem2 = transform.GetComponentInChildren<InventoryItem>();
                inventoryItem3.parentAfterDrag = transform;
                gamem.GetComponent<InventoryManager>().money += inventoryItem3.costItem;
                inventoryItem3.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem3.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
            else
            {
                inventoryItem3.parentAfterDrag = transform;
            }
        }
        //buy
        else if (eventData.pointerDrag.GetComponent<InventoryItem>().parentAfterDrag.transform.GetComponent<InventorySlot>().shopSlot == true)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            //buy on free slot
            if (transform.childCount == 0 && gamem.GetComponent<InventoryManager>().money >= inventoryItem.costItem)
            {
                inventoryItem.parentAfterDrag = transform;

                if (inventoryItem.parentAfterDrag.transform.GetComponent<InventorySlot>().shopSlot == false)
                {
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.costItem;
                }
            }
            //swap items
            else if (transform.childCount == 1 && transform.GetComponentInChildren<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
            {
                Debug.Log("Its not free slot");
            }
            //connect items
            else if (eventData.pointerDrag.GetComponentInChildren<InventoryItem>().item.isStack == true)
            {
                InventoryItem inventoryItem2 = transform.GetComponentInChildren<InventoryItem>();

                if (inventoryItem2 == null && transform.GetComponentInParent<InventorySlot>().shopSlot == false && gamem.GetComponent<InventoryManager>().money >= inventoryItem.item.costTosell)
                {
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.item.costTosell;
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.RefreshCount();
                }
                else if (inventoryItem2 == null && transform.GetComponentInParent<InventorySlot>().shopSlot == true)
                {
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.RefreshCount();
                }
                else if (inventoryItem2 != null && transform.GetComponentInParent<InventorySlot>().shopSlot == false && gamem.GetComponent<InventoryManager>().money >= inventoryItem.item.costTosell)
                {
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.costItem;
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.count += inventoryItem2.count;
                    inventoryItem.RefreshCount();
                    Destroy(inventoryItem2.gameObject);
                }
                else if (inventoryItem2 != null && transform.GetComponentInParent<InventorySlot>().shopSlot == true)
                {
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.count += inventoryItem2.count;
                    inventoryItem.RefreshCount();
                    Destroy(inventoryItem2.gameObject);
                }
                else if(inventoryItem2 == null) //block all
                {
                    InventorySlot slotParent = GetComponent<InventorySlot>();
                    if(eventData.pointerDrag.GetComponentInChildren<InventoryItem>().count == 1)
                    {
                        slotParent = inventoryItem.GetComponent<InventoryItem>().parentAfterDrag.GetComponentInParent<InventorySlot>();
                        slotParent.GetComponentInChildren<InventoryItem>().count += 1;
                        slotParent.GetComponentInChildren<InventoryItem>().RefreshCount();
                        Destroy(inventoryItem.gameObject);
                        Debug.Log("You don't have money farmer");
                    }
                    else
                    {
                        Debug.Log("You don't have money farmer");
                    }
                }
                else if (inventoryItem2 != null) //block 1 slot
                {
                    InventorySlot slotParent = GetComponent<InventorySlot>();
                    slotParent = inventoryItem.GetComponent<InventoryItem>().parentAfterDrag.GetComponentInParent<InventorySlot>();
                    slotParent.GetComponentInChildren<InventoryItem>().count += 1;
                    slotParent.GetComponentInChildren<InventoryItem>().RefreshCount();
                    Destroy(inventoryItem.gameObject);
                    Debug.Log("You don't have money farmer");
                }
            }
            else if(inventoryItem.parentAfterDrag.transform.GetComponentInChildren<InventorySlot>().shopSlot == true && transform.GetComponentInChildren<InventorySlot>().shopSlot == true)
            {
                inventoryItem.parentAfterDrag = transform;
            }
            else
            {
                Debug.Log("You don't have money farmer");
            }
        }
        //rushbin
        else if(rushBin == true)
        {
            Destroy(eventData.pointerDrag);
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
            else if (transform.childCount == 1 && transform.GetComponentInChildren<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item) // swap items
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetComponentInChildren<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
                inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
            }
            //connect items
            else if (transform.GetComponentInChildren<InventoryItem>().item.isStack == true && transform.GetComponentInChildren<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetComponentInChildren<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
                inventoryItem.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
        }
    }
}