using UnityEngine;

[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Consumeable")]
public class ConsumeableClass : Item
{
    public float healthAdded;
    public override Item GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override ConsumeableClass GetConsumeable() { return this; }
}