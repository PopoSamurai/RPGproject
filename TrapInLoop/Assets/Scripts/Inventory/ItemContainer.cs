using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ItemContainer : MonoBehaviour
{
    public ItemData itemType;
    public ItemContainer(ItemData itemData)
    {
        itemType = itemData;
    }
}
