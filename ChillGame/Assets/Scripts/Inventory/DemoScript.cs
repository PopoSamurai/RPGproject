using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickUp;

    public void PickUpItem(int id)
    {
       bool result = inventoryManager.AddItem(itemsToPickUp[id]);
        if (result == true)
        {
            //add item to eq
        }
        else
        {
            //eq is full
        }
    }
}
