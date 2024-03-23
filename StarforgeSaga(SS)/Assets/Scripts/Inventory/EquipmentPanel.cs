using System;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] Transform eqSlotParent;
    [SerializeField] EquipmentSlot[] eqSlot;
    public event Action<Item> OnItemRightClickedEvent;
    private void Awake()
    {
        for (int i = 0; i < eqSlot.Length; i++)
        {
            eqSlot[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }
    private void OnValidate()
    {
        eqSlot = eqSlotParent.GetComponentsInChildren<EquipmentSlot>();
    }
    public bool AddItem(EquippableItem item, out EquippableItem previousItem)
    {
        for(int i = 0; i < eqSlot.Length; i++)
        {
            if (eqSlot[i].Type == item.type)
            {
                previousItem = (EquippableItem)eqSlot[i].Item;
                eqSlot[i].Item = item;
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    public bool RemoveItem(EquippableItem item)
    {
        for (int i = 0; i < eqSlot.Length; i++)
        {
            if (eqSlot[i].Item == item)
            {
                eqSlot[i].Item = null;
                return true;
            }
        }
        return false;
    }
}
