using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public DynamicInv inventory;
    public ItemDisplay[] slots;

    private void Start()
    {
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].UpdateItemDisplay(inventory.items[i].itemType.icon, i);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
    public void DropItem(int itemIndex)
    {
        GameObject droppedItem = new GameObject();
        droppedItem.AddComponent<Rigidbody>();
        droppedItem.AddComponent<InstanceItemContainer>().item = inventory.items[itemIndex];
        GameObject itemModel = Instantiate(inventory.items[itemIndex].itemType.model, droppedItem.transform);
        inventory.items.RemoveAt(itemIndex);
        UpdateInventory();
    }
}