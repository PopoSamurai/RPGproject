using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [SerializeField] private SlotClass[] startingItems;
    private SlotClass[] items;
    private GameObject[] slots;
    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;
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
        Add(itemToAdd);
        Remove(itemToRemove);
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) // click
        {
            //BeginItemMove();
            Debug.Log(GetClosestSlot());
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
    public bool Add(ItemClass item)
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
                    items[i].AddItem(item, 1);
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
    //private bool BeginItemMove()
    //{
    //    originalSlot = GetClosestSlot();
    //    if (originalSlot == null || originalSlot.GetItem() == null)
    //        return false;   //no item to move

    //    movingSlot = new SlotClass(originalSlot);
    //    originalSlot.Clear();
    //    RefreshUI();
    //    return true;
    //}
    private SlotClass GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
                return items[i];
        }
        return null;
    }
    #endregion Move slots
}