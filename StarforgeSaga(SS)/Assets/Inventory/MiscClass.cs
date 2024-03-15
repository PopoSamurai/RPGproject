using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName = "Item/Misc")]
public class MiscClass : ItemClass
{
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return this; }
    public override ComsumableClass GetComsuable() { return null; }
}