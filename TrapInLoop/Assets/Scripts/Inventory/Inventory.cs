using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemContainer> items = new();
    public void AddItem(ItemContainer itemToAdd)
    {
        items.Add(itemToAdd);
    }
    public void RemoveItem(ItemContainer itemToRemove)
    {
        items.Remove(itemToRemove);
    }
}
