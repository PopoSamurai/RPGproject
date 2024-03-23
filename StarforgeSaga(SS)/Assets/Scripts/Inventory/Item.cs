using UnityEngine;

[CreateAssetMenu(fileName = "new item", menuName = "Prefab Item/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public bool isStackable;
    public int count = 0;
    //quality item
    //stats
}
