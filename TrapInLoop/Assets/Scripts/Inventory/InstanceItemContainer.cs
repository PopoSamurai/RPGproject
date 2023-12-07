using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceItemContainer : MonoBehaviour
{
    public ItemContainer item;
    public ItemContainer TakeItem()
    {
        Destroy(gameObject);
        return item;
    }
}
