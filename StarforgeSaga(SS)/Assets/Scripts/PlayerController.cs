using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InventoryManager inventory;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            //use item
            if(inventory.selectedItem != null)
            inventory.selectedItem.Use(this);
        }
    }
}