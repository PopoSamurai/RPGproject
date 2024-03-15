using UnityEngine;

[System.Serializable]
public class SlotClass
{
    [SerializeField] private ItemClass item = null;
    [SerializeField] private int quantity = 0;
    public SlotClass()
    {
        item = null;
        quantity = 0;
    }
    public SlotClass(ItemClass _item, int _quality)
    {
        item = _item;
        quantity = _quality;
    }
    public SlotClass(SlotClass slot)
    {
        this.item = slot.GetItem();
        this.quantity = slot.GetQuantity();
    }
    public void Clear()
    {
        this.item = null;
        this.quantity = 0;
    }
    public ItemClass GetItem() { return item; }
    public int GetQuantity() { return quantity; }
    public void AddQuantity(int _quality) { quantity += _quality; }
    public void SubQuantity(int _quality) { quantity -= _quality; }
    public void AddItem(ItemClass item, int quality)
    {
        this.item = item;
        this.quantity = quality;
    }
}
