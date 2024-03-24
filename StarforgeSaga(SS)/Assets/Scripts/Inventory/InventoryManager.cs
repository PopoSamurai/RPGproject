using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject cursor;

    [SerializeField] private GameObject slotHolder;
    [SerializeField] private Item itemToAdd;
    [SerializeField] private Item itemToRemove;

    [SerializeField] private Slot[] startingItems;
    private Slot[] items;
    private GameObject[] slots;
    private Slot movingSlot;
    private Slot tempSlot;
    private Slot OriginalSlot;
    bool isMoving;
    private void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new Slot[slots.Length];
        for(int i = 0; i < items.Length; i++)
        {
            items[i] = new Slot();
        }
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }
        //set all slots
        for (int i = 0; i < slotHolder.transform.childCount; i++)
            slots[i] = slotHolder.transform.GetChild(i).gameObject;

        Add(itemToAdd, 1);
        Remove(itemToRemove);
        RefreshUI();
    }
    private void Update()
    {
        cursor.SetActive(isMoving);
        cursor.transform.position = Input.mousePosition;
        if(isMoving)
            cursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;

        if (Input.GetMouseButtonDown(0)) //click
        {
            if(isMoving)
            {
                EndItemMove();
            }
            else
                BeginItemMove();
        }
    }
    #region Inventory
    public void RefreshUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;

                if (items[i].GetItem().isStack)
                    slots[i].transform.GetChild(1).GetComponent<Text>().text = items[i].GetCount() + "";
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
    public bool Add(Item item, int count)
    {
        Slot slot = Containts(item);
        if (slot != null && slot.GetItem().isStack)
            slot.AddCount(1);
        else
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null)
                {
                    items[i].AddItem(item, count);
                    break;
                }
            }
        }

        RefreshUI();
        return true;
    }
    public bool Remove(Item item)
    {
        Slot temp = Containts(item);
        if (temp != null)
        {
            if (temp.GetCount() > 1)
                temp.SubCount(1);
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
    public Slot Containts(Item item)
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item)
                return items[i];
        }
        return null;
    }
    #endregion Inventory
    #region Move
    private bool BeginItemMove()
    {
        OriginalSlot = GetClosestSlot();
        if (OriginalSlot == null || OriginalSlot.GetItem() == null)
            return false;

        movingSlot = new Slot(OriginalSlot);
        OriginalSlot.Clear();
        isMoving = true;
        RefreshUI();
        return true;
    }
    private bool EndItemMove()
    {
        OriginalSlot = GetClosestSlot();

        if (OriginalSlot == null)
        {
            Add(movingSlot.GetItem(), movingSlot.GetCount());
            movingSlot.Clear();
        }
        else
        {
            if (OriginalSlot.GetItem() != null)
            {
                if (OriginalSlot.GetItem() == movingSlot.GetItem())
                {
                    if (OriginalSlot.GetItem().isStack)
                    {
                        OriginalSlot.AddCount(movingSlot.GetCount());
                        movingSlot.Clear();
                    }
                    else
                        return false;
                }
                else
                {
                    tempSlot = new Slot(OriginalSlot); // a = b
                    OriginalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetCount()); // b = c
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetCount()); // a = c
                    RefreshUI();
                    return true;
                }
            }
            else
            {
                OriginalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetCount());
                movingSlot.Clear();
            }
        }

        isMoving = false;
        RefreshUI();
        return true;
    }
    private Slot GetClosestSlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
                return items[i];
        }
        return null;
    }
    #endregion Move
}