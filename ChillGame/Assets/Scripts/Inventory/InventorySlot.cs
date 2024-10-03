using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;
public enum SlotType
{
    None,
    Sword,
    water,
    fishRod,
    seed,
    crop
}
public class InventorySlot : MonoBehaviour, IDropHandler/*, IPointerDownHandler*/
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

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        // Handling item in inventory slot
        if (eqSlot)
        {
            HandleInventorySlot(draggedItem);
        }
        // Handling selling item to the shop
        else if (shopSlot && !draggedItem.parentAfterDrag.GetComponent<InventorySlot>().shopSlot)
        {
            HandleSellItem(draggedItem);
        }
        // Buying item from the shop
        else if (draggedItem.parentAfterDrag.GetComponent<InventorySlot>().shopSlot)
        {
            HandleBuyItem(draggedItem);
        }
        // Handling rush bin (delete item)
        else if (rushBin)
        {
            Destroy(eventData.pointerDrag.gameObject);
        }
        else
        {
            HandleGeneralSlot(draggedItem);
        }
    }

    private void HandleInventorySlot(InventoryItem draggedItem)
    {
        if (transform.childCount == 0)
        {
            if ((int)type == (int)draggedItem.item.type)
            {
                draggedItem.parentAfterDrag = transform;
            }
        }
        else if (transform.childCount == 1)
        {
            InventoryItem existingItem = transform.GetChild(0).GetComponent<InventoryItem>();

            if (existingItem.item != draggedItem.item && (int)type == (int)draggedItem.item.type)
            {
                SwapItems(draggedItem, existingItem);
            }
            else if (existingItem.item.isStack && existingItem.count + draggedItem.count <= 65)
            {
                StackItems(draggedItem, existingItem);
            }
        }
    }

    private void HandleSellItem(InventoryItem draggedItem) //w sklepie
    {
        InventoryItem existingItem = transform.GetComponentInChildren<InventoryItem>();

        if (transform.childCount == 0 && shopSlot) //jak pusty slot
        {
            draggedItem.parentAfterDrag = transform;
            gamem.GetComponent<InventoryManager>().money += draggedItem.costItem;
            Debug.Log("Item sold");
        }
        else if (shopSlot && existingItem.item == draggedItem.item && transform.GetComponentInChildren<InventoryItem>().item.isStack && transform.GetComponentInChildren<InventoryItem>().count + draggedItem.count <= 65)
        {
            StackItems(draggedItem, transform.GetComponentInChildren<InventoryItem>());
            gamem.GetComponent<InventoryManager>().money += draggedItem.costItem;
        }
        else
        {
            if (existingItem.item == draggedItem.item && shopSlot)
            {
                StackItems(draggedItem, existingItem);
            }
            else
            {
                //dodaj count do poprzedniego
                draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().count += draggedItem.count;
                draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                Destroy(draggedItem.gameObject);
                Debug.Log("Slot is not free to sell");
            }
        }
    }

    private void HandleBuyItem(InventoryItem draggedItem)
    {
        InventoryManager inventoryManager = gamem.GetComponent<InventoryManager>();
        InventoryItem existingItem = transform.GetComponentInChildren<InventoryItem>();

        if (transform.childCount == 0)
        {
            if (inventoryManager.money >= draggedItem.costItem)
            {
                draggedItem.parentAfterDrag = transform;
                inventoryManager.money -= draggedItem.costItem;
            }
            else if (shopSlot)
            {
                draggedItem.parentAfterDrag = transform;
            }

        }
        else if (transform.childCount == 1 && transform.GetComponentInChildren<InventoryItem>().item.isStack && inventoryManager.money >= draggedItem.costItem)
        {
            if (existingItem.item == draggedItem.item && existingItem.item.isStack)
            {
                StackItems(draggedItem, transform.GetComponentInChildren<InventoryItem>());

                if (!shopSlot)
                    inventoryManager.money -= draggedItem.costItem;
            }
            else
            {
                //dodaj count do poprzedniego
                if (!shopSlot)
                    draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().count += draggedItem.count;
                draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                Destroy(draggedItem.gameObject);
                Debug.Log("You don't have a money farmer");
            }
        }
        else
        {                    
            if (existingItem.item == draggedItem.item && inventoryManager.money >= draggedItem.costItem)
            {
                StackItems(draggedItem, existingItem);
                if (!shopSlot)
                    inventoryManager.money -= draggedItem.costItem;
            }
            else if(existingItem.item != draggedItem.item && shopSlot)
            {
                SwapItems(draggedItem, existingItem);
            }
            else if (existingItem.item == draggedItem.item && existingItem.item.isStack && shopSlot)
            {
                StackItems(draggedItem, existingItem);
            }
            else
            {
                //dodaj count do poprzedniego
                if (!shopSlot)
                    draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().count += draggedItem.count;
                draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                Destroy(draggedItem.gameObject);
                Debug.Log("You don't have a money farmer");
            }
        }
    }
    private void HandleGeneralSlot(InventoryItem draggedItem) //w eq
    {
        if (transform.childCount == 0)
        {
            draggedItem.parentAfterDrag = transform;
        }
        else
        {
            InventoryItem existingItem = transform.GetComponentInChildren<InventoryItem>();

            if (existingItem.item != draggedItem.item && !draggedItem.isSplitting)
            {
                SwapItems(draggedItem, existingItem);
            }
            else if (existingItem.item.isStack && !draggedItem.isSplitting)
            {
                if (existingItem.count + draggedItem.count <= 65)
                {
                    StackItems(draggedItem, existingItem);
                }
                else
                {
                    SwapItems(draggedItem, existingItem);
                }
            }
            else
            {
                if(existingItem.item == draggedItem.item && existingItem.item.isStack)
                {
                    StackItems(draggedItem, existingItem);
                }
                else if(existingItem.item == draggedItem.item && !existingItem.item.isStack)
                {
                    SwapItems(draggedItem, existingItem);
                }
                else
                {
                    //dodaj count do poprzedniego
                    draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().count += draggedItem.count;
                    draggedItem.firstPos.GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                    Destroy(draggedItem.gameObject);
                    Debug.Log("It's not a free slot");
                }
            }
        }
    }

    private void SwapItems(InventoryItem draggedItem, InventoryItem existingItem)
    {
        draggedItem.parentAfterDrag = transform;
        existingItem.parentAfterDrag = draggedItem.firstPos.transform;
        existingItem.transform.SetParent(draggedItem.firstPos.transform);
    }

    private void StackItems(InventoryItem draggedItem, InventoryItem existingItem)
    {
        draggedItem.parentAfterDrag = transform;
        existingItem.count += draggedItem.count;
        existingItem.RefreshCount();
        Destroy(draggedItem.gameObject);
    }
}