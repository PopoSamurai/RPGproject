using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DynamicInv : ScriptableObject
{
    public int maxItems = 44;
    public List<ItemContainer> items = new();
    public bool addItem(ItemContainer itemToAdd)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;
                return true;
            }
        }
        if(items.Count < maxItems)
        {
            items.Add(itemToAdd);
            return true;
        }
        Debug.Log("Brak miejsca w eq");
        return false;
    }
}
