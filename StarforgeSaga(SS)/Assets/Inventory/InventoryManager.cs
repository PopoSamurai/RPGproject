using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject itemCursor;
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private GameObject hotbarSlotHolder; //holder eq
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [SerializeField] private SlotClass[] startingItems;
    private SlotClass[] items;
    private SlotClass[] hotbar; //eq slots
    private GameObject[] slots;

    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;
    bool isMovingItem;
    private void Start()
    {
        slots = new GameObject[slotsHolder.transform.childCount];
        items = new SlotClass[slots.Length];
        for(int i = 0; i < items.Length; i++)
        {
            items[i] = new SlotClass();
        }
        //eq for start
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }

        //set all the slots
        for (int i = 0; i < slotsHolder.transform.childCount; i++)
            slots[i] = slotsHolder.transform.GetChild(i).gameObject;

        RefreshUI();
        Add(itemToAdd, 1);
        Remove(itemToRemove);
    }
    private void Update()
    {
        itemCursor.SetActive(isMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        if (isMovingItem)
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;

        if(Input.GetMouseButtonDown(0)) // click left
        {
            if(isMovingItem)
            {
                //end item move
                EndItemMove();
            }
            else BeginItemMove();
        }
        else if (Input.GetMouseButtonDown(1)) //click right
        {
            if (isMovingItem)
            {
                EndItemMove_Half();
            }
            else
                BeginItemMove_Half();
        }
    }

    #region Inventory Options
    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try 
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                if (items[i].GetItem().isStackable)
                    slots[i].transform.GetChild(1).GetComponent<Text>().text = items[i].GetQuantity() + "";
                else
                    slots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
            catch 
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    } 
    public bool Add(ItemClass item, int quantity)
    {
        SlotClass slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
            slot.AddQuantity(1);
        else
        {
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null) // pusty slot
                {
                    items[i].AddItem(item, quantity);
                    break;
                }
            }
        }
        RefreshUI();
        return true;
    }
    public bool Remove(ItemClass item)
    {
        SlotClass temp = Contains(item);
        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
                temp.SubQuantity(1);
            else
            {
                int slotToRemoveIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }
                items[slotToRemoveIndex].Clear();
            }
        }
        else
        {
            return false;
        }
        RefreshUI();
        return true;
    }
    public SlotClass Contains(ItemClass item)
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item)
                return items[i];
        }
        return null;
    }
    #endregion Inventory Options

    #region Move slots
    private bool BeginItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return false;   //no item to move

        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }
    private bool BeginItemMove_Half()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return false;   //no item to move

        movingSlot = new SlotClass(originalSlot.GetItem(), Mathf.CeilToInt(originalSlot.GetQuantity() / 2f));
        originalSlot.SubQuantity(Mathf.CeilToInt(originalSlot.GetQuantity() / 2f));
        if(originalSlot.GetQuantity() == 0)
            originalSlot.Clear();

        isMovingItem = true;
        RefreshUI(); 
        return true;
    }
    private bool EndItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
        {
            Add(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear();
        }
        else
        {
            if (originalSlot.GetItem() != null)
            {
                if (originalSlot.GetItem() == movingSlot.GetItem()) //the same item
                {
                    if (originalSlot.GetItem().isStackable)
                    {
                        originalSlot.AddQuantity(movingSlot.GetQuantity());
                        movingSlot.Clear();
                    }
                    else
                        return false;
                }
                else
                {
                    tempSlot = new SlotClass(originalSlot); // a = b
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity()); //b = c
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity()); // a = c
                    RefreshUI();
                    return true;
                }
            }
            else //place item
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.Clear();
            }
        }
        isMovingItem = false;
        RefreshUI();
        return true;
    }
    private bool EndItemMove_Half()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
            return false;
        else if (originalSlot.GetItem() != null && originalSlot.GetItem() != movingSlot.GetItem())
            return false;

        movingSlot.SubQuantity(1);
        if(originalSlot.GetItem() != null && originalSlot.GetItem() == movingSlot.GetItem())
            originalSlot.AddQuantity(1);
        else
            originalSlot.AddItem(movingSlot.GetItem(), 1);

        if (movingSlot.GetQuantity() < 1)
        {
            isMovingItem = false;
            movingSlot.Clear();
        }
        else
            isMovingItem = true;

        RefreshUI();
        return true;
    }
    private SlotClass GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
            {
                return items[i];
            }
        }
        return null;
    }
    #endregion Move slots
}