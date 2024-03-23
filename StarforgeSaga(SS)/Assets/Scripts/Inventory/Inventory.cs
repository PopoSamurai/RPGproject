using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<Item> items;
    [SerializeField] Transform itemParent;
    [SerializeField] Slot[] slots;
    public event Action<Item> OnItemRightClickedEvent;
    private void Awake()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }
    private void OnValidate()
    {
        if(itemParent != null)
            slots = itemParent.GetComponentsInChildren<Slot>();

        RefreshUI();
    }
    private void RefreshUI()
    {
        int i = 0;
        for(; i < items.Count && i < slots.Length; i++)
        {
            slots[i].Item = items[i];
        }

        for(; i < slots.Length; i++)
        {
            slots[i].Item = null;
        }
    }
    public bool AddItem(Item item)
    {
        if(IsFull())
            return false;

        items.Add(item);
        RefreshUI();
        return true;
    }
    public bool RemoveItem(Item item)
    {
        if(items.Remove(item))
        {
            RefreshUI();
            return true;
        }
        return false;
    }
    public bool IsFull()
    {
        return items.Count >= slots.Length;
    }
}
