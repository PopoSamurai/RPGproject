using UnityEngine;

public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public bool isStack = true;
    public int stackSoze = 64;
    public SlotType slotType;
    public virtual void Use(PlayerController controller)
    {
        Debug.Log("Used Item");
    }
    public virtual Item GetItem() { return this; }
    public virtual ToolClass GetTool() { return null; }
    public virtual MiscClass GetMisc() { return null; }
    public virtual ConsumeableClass GetConsumeable() { return null; }
}