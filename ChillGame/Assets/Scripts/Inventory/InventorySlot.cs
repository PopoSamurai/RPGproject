using UnityEditor.Experimental.GraphView;
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
                Debug.Log("connect");
                //rozdzielanie itemow w sklepie napraw!
                InventoryItem inventoryItem2 = transform.GetComponentInChildren<InventoryItem>();
                inventoryItem3.parentAfterDrag = transform;
                gamem.GetComponent<InventoryManager>().money += inventoryItem3.costItem;
                inventoryItem3.GetComponent<InventoryItem>().count += inventoryItem2.GetComponent<InventoryItem>().count;
                inventoryItem3.GetComponent<InventoryItem>().RefreshCount();
                Destroy(inventoryItem2.gameObject);
            }
            else
            {
                Debug.Log("else");
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
                Debug.Log("7");
                inventoryItem.parentAfterDrag = transform;

                if (inventoryItem.parentAfterDrag.transform.GetComponent<InventorySlot>().shopSlot == false)
                {
                    Debug.Log("aaa");
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.costItem;
                }
            }
            //swap items
            else if (transform.childCount == 1 && transform.GetComponentInChildren<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item)
            {
                Debug.Log("Its not free slot 6");
            }
            //connect items
            else if (eventData.pointerDrag.GetComponentInChildren<InventoryItem>().item.isStack == true)
            {
                InventoryItem inventoryItem2 = transform.GetComponentInChildren<InventoryItem>();

                if (inventoryItem2 == null && transform.GetComponentInParent<InventorySlot>().shopSlot == false && gamem.GetComponent<InventoryManager>().money >= inventoryItem.item.costTosell)
                {
                    Debug.Log("1");
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.item.costTosell;
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.RefreshCount();
                }
                else if (inventoryItem2 == null && transform.GetComponentInParent<InventorySlot>().shopSlot == true)
                {
                    Debug.Log("2");
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.RefreshCount();
                }
                else if (inventoryItem2 != null && transform.GetComponentInParent<InventorySlot>().shopSlot == false && gamem.GetComponent<InventoryManager>().money >= inventoryItem.item.costTosell)
                {
                    //tu
                    Debug.Log("3");
                    gamem.GetComponent<InventoryManager>().money -= inventoryItem.item.costTosell;
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.count += inventoryItem2.count;
                    inventoryItem.RefreshCount();
                    Destroy(inventoryItem2.gameObject);
                }
                else if (inventoryItem2 != null && transform.GetComponentInParent<InventorySlot>().shopSlot == true)
                {
                    Debug.Log("4");
                    inventoryItem.parentAfterDrag = transform;
                    inventoryItem.count += inventoryItem2.count;
                    inventoryItem.RefreshCount();
                    Destroy(inventoryItem2.gameObject);
                }
                else
                {
                    Debug.Log("You don't have money farmer 2");
                }
            }
            else if(inventoryItem.parentAfterDrag.transform.GetComponentInChildren<InventorySlot>().shopSlot == true && transform.GetComponentInChildren<InventorySlot>().shopSlot == true)
            {
                Debug.Log("fix");
                inventoryItem.parentAfterDrag = transform;
            }
            else
            {
                Debug.Log("You don't have money farmer 3");
            }
        }
        //rushbin
        else if(rushBin == true)
        {
            Debug.Log("rushbin");
            Destroy(eventData.pointerDrag);
        }
        else
        {
            //change item position
            if (transform.childCount == 0)
            {
                Debug.Log("change item pos");
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
            }
            //swap items
            else if (transform.childCount == 1 && transform.GetComponentInChildren<InventoryItem>().item != eventData.pointerDrag.GetComponent<InventoryItem>().item) // swap items
            {
                Debug.Log("normal swap");
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                InventoryItem inventoryItem2 = transform.GetComponentInChildren<InventoryItem>();
                inventoryItem.parentAfterDrag = transform;
                inventoryItem2.parentAfterDrag = inventoryItem.firstPos.transform;
                inventoryItem2.transform.SetParent(inventoryItem.firstPos.transform);
            }
            //connect items
            else if (transform.GetComponentInChildren<InventoryItem>().item.isStack == true && transform.GetComponentInChildren<InventoryItem>().count + eventData.pointerDrag.GetComponent<InventoryItem>().count < 65)
            {
                Debug.Log("normal connect");
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