using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName = "Item/Comsumeable")]
public class ComsumableClass : ItemClass
{
    [Header("Comsumable")]
    public float healthAdded;
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override ComsumableClass GetComsuable() { return this; }
}
