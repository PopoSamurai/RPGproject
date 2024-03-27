using UnityEngine;

[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Misc")]
public class MiscClass : Item
{
    public override void Use(PlayerController controller)
    {
        
    }
    public override MiscClass GetMisc() { return this; }
}