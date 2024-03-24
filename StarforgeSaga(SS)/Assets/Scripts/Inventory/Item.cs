using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public bool isStack = true;
    public abstract Item GetItem();
    public abstract ToolClass GetTool();
    public abstract MiscClass GetMisc();
    public abstract ConsumeableClass GetConsumeable();
}