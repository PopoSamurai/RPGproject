using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/new Item")]
public class Item : ScriptableObject
{
    public TileBase tile;
    public Sprite icon;
    public ItemType itemtype;
    public Vector2Int range = new Vector2Int(5, 4);
    public bool isStack = true;
    public int costTosell = 0;
    public enum ItemType
    {
        None,
        Sword,
        water,
        fishRod,
        seed,
        crop
    }
}