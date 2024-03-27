using UnityEngine;
[System.Serializable]
public class Slot
{
    [SerializeField] public Item item { get; private set; } = null;
    [SerializeField] public int count { get; private set; } = 0;
    public SlotType slotType { get; private set; } = SlotType.def;
    public Slot()
    {
        item = null;
        count = 0;
    }
    public Slot(Item _item, int _count)
    {
        this.item = _item;
        this.count = _count;
    }
    public Slot(Slot slot)
    {
        this.item = slot.GetItem();
        this.count = slot.GetCount();
    }
    public void Clear()
    {
        this.item = null;
        this.count = 0;
    }
    public Item GetItem() { return item; }
    public int GetCount() { return count; }
    public void AddCount(int _count) { count += _count; }
    public void SubCount(int _count) 
    { 
        count -= _count; 
        if(count <= 0)
        {
            Clear();
        }
    }
    public void AddItem(Item _item, int _count)
    {
        this.item = _item;
        this.count = _count;
    }
}
