using UnityEngine;

[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Consumeable")]
public class ConsumeableClass : Item
{
    public float healthAdded;
    public override void Use(PlayerController controller)
    {
        base.Use(controller);
        Debug.Log("Eat Consumeable");
        controller.inventory.UseSelected();
    }
    public override ConsumeableClass GetConsumeable() { return this; } 
}