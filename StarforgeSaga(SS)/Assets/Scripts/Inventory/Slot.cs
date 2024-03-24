using UnityEngine;
[System.Serializable]
public class Slot
{
    [SerializeField] private Item item;
    [SerializeField] private int count;
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
    public void SubCount(int _count) { count -= _count; }
    public void AddItem(Item _item, int _count)
    {
        this.item = _item;
        this.count = _count;
    }
}
