using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/new Item")]
public class Item : ScriptableObject
{
    public TileBase tile;
    public Sprite icon;
    public ItemType type;
    public ActionType action;
    public Vector2Int range = new Vector2Int(5, 4);
    public bool isStack = true;
    public enum ItemType
    {
        Tool,
        Eat
    }
    public enum ActionType
    {
        water,
        seed,
        crop,
        attack,
        fish,
        mine,
        dig,
        cropTree
    }
}
