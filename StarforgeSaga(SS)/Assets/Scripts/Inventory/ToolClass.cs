using UnityEngine;
[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Tool")]
public class ToolClass : Item
{
    public ToolType type;
    public enum ToolType
    {
        weampon,
        hammer,
        pickaxe,
        water
    }
    public override Item GetItem() { return this; }
    public override ToolClass GetTool() { return this; }
    public override MiscClass GetMisc() { return null; }
    public override ConsumeableClass GetConsumeable() { return null; }
}