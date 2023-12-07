using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public DynamicInv inventory;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out InstanceItemContainer foundItem))
        {
            inventory.addItem(foundItem.TakeItem());
        }
    }
}
