using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipeClass> craftingRecipes = new List<CraftingRecipeClass>();
    [SerializeField] private GameObject cursor;

    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject hotbarHolder;
    [SerializeField] private Item itemToAdd;
    [SerializeField] private Item itemToRemove;

    [SerializeField] private Slot[] startingItems;
    private Slot[] items;

    private GameObject[] slots;
    private GameObject[] hotBarSlots;

    private Slot movingSlot;
    private Slot tempSlot;
    private Slot OriginalSlot;
    bool isMoving;

    [SerializeField] private GameObject hotbarSelector;
    [SerializeField] private int selectedSlotIndex = 0;
    public Item selectedItem;
    private void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new Slot[slots.Length];

        hotBarSlots = new GameObject[hotbarHolder.transform.childCount];

        for (int i = 0; i < hotBarSlots.Length; i++)
        {
            hotBarSlots[i] = hotbarHolder.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < items.Length; i++)
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

        RefreshUI();

        Add(itemToAdd, 1);
        Remove(itemToRemove);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)) //hamdle crafting
            Craft(craftingRecipes[0]);

        cursor.SetActive(isMoving);
        cursor.transform.position = Input.mousePosition;
        if(isMoving)
            cursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;

        if (Input.GetMouseButtonDown(0)) //click
        {
            if(isMoving)
            {
                EndItemMove(); //end move
            }
            else
                BeginItemMove();
        }
        else if(Input.GetMouseButtonDown(1)) //serparate
        {
            if (isMoving)
            {
                EndItemMove_Single(); //end move
            }
            else
                BeginItemMove_Half();
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0) //scroll up
        {
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex + 1, 0, hotBarSlots.Length);
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0) //scroll down
        {
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex - 1, 0, hotBarSlots.Length);
        }
        hotbarSelector.transform.position = hotBarSlots[selectedSlotIndex].transform.position;
        selectedItem = items[selectedSlotIndex + (hotBarSlots.Length * 3)].GetItem();
    }
    private void Craft(CraftingRecipeClass recipe)
    {
        if (recipe.CanCraft(this))
            recipe.Craft(this);
        else
            Debug.Log("Can craft item");
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
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
        RefreshHotBar();
    }
    public void RefreshHotBar()
    {
        for (int i = 0; i < hotBarSlots.Length; i++)
        {
            try
            {
                hotBarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotBarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i + (hotBarSlots.Length * 3)].GetItem().itemIcon;

                if (items[i + (hotBarSlots.Length * 3)].GetItem().isStack)
                    hotBarSlots[i].transform.GetChild(1).GetComponent<Text>().text = items[i + (hotBarSlots.Length * 3)].GetCount() + "";
                else
                    hotBarSlots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
            catch
            {
                hotBarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                hotBarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotBarSlots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    }
    public bool Add(Item item, int count)
    {
        Slot slot = Containts(item);
        if (slot != null && slot.GetItem().isStack)
            slot.AddCount(count);
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
        isMoving = false;
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
    public bool Remove(Item item, int count)
    {
        Slot temp = Containts(item);
        if (temp != null)
        {
            if (temp.GetCount() > 1)
                temp.SubCount(count);
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
    public void UseSelected()
    {
        items[selectedSlotIndex + (hotBarSlots.Length * 3)].SubCount(1);
        RefreshUI();
    }
    public bool isFull()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == null)
                return false;
        }
        return false;
    }
    public Slot Containts(Item item)
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item)
            {
                return items[i];
            }
        }
        return null;
    }
    public bool Containts(Item item, int count)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item && items[i].GetCount() >= count)
                return true;
        }
        return false;
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
    private bool BeginItemMove_Half()
    {
        OriginalSlot = GetClosestSlot();
        if (OriginalSlot == null || OriginalSlot.GetItem() == null)
            return false;

        movingSlot = new Slot(OriginalSlot.GetItem(), Mathf.CeilToInt(OriginalSlot.GetCount() / 2f));
        OriginalSlot.SubCount(Mathf.CeilToInt(OriginalSlot.GetCount() / 2f) );
        if(OriginalSlot.GetCount() == 0)
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
    private bool EndItemMove_Single() 
    {
        OriginalSlot = GetClosestSlot();

        if (OriginalSlot == null)
            return false;
        if(OriginalSlot.GetItem() != null && OriginalSlot.GetItem() != movingSlot.GetItem())
            return false;

        movingSlot.SubCount(1);
        if(OriginalSlot.GetItem() != null && OriginalSlot.GetItem() == movingSlot.GetItem())
            OriginalSlot.AddCount(1);
        else
            OriginalSlot.AddItem(movingSlot.GetItem(), 1);

        if (movingSlot.GetCount() < 1)
        {
            isMoving = false;
            movingSlot.Clear();
        }
        else
            isMoving = true;

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