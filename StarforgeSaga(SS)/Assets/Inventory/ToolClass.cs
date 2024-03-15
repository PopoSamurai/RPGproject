using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName = "Item/Tool")]
public class ToolClass : ItemClass
{
    [Header("Tool")]
    public ToolType toolType;
    public enum ToolType
    {
        weampon,
        nextWeampon,
        helmet,
        boots,
        gloves,
        armor,
        ring,
        neackle
    }

    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return this; }
    public override MiscClass GetMisc() { return null; }
    public override ComsumableClass GetComsuable() { return null; }
}