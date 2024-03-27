using UnityEngine;
[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/SpawnSword")]
public class SpawnSword : ToolClass
{
    public GameObject SpawnObject;
    public override void Use(PlayerController controller)
    {
        base.Use(controller);
        Object.Instantiate(SpawnObject, controller.transform.position, Quaternion.identity);
    }
}