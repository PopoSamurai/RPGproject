using UnityEngine;

public class Shop : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickUp;
    public void OpenShop()
    {
        for (int i = 0; i < itemsToPickUp.Length; i++)
        {
            PickUpItem2(i);
        }
    }
    public void resetShop()
    {
        for (int i = 0; i < inventoryManager.shopSlot.Length; i++)
        {
            InventorySlot slot = inventoryManager.shopSlot[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                Debug.Log("chold is null");
            }
            else
            {
                Destroy(itemInSlot.gameObject);
            }
        }
    }
    public void PickUpItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPickUp[id]);
        if (result == true)
        {
            Debug.Log("Item added");
        }
        else
        {
            Debug.Log("EQ is full");
        }
    }
    public void PickUpItem2(int id)
    {
        bool result = inventoryManager.AddShop(itemsToPickUp[id]);
        if (result == true)
        {
            Debug.Log("Item added");
        }
        else
        {
            Debug.Log("EQ is full");
        }
    }
}
