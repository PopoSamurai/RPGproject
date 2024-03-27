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
    public override void Use(PlayerController controller)
    {
        base.Use(controller);
        Debug.Log("Swing tool");
    }
    public override ToolClass GetTool() { return this; }
}