using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/new Item")]
public class Item : ScriptableObject
{
    public string nameItem;
    public TileBase tile;
    public Sprite icon;
    public SlotType type;
    public Vector2Int range = new Vector2Int(5, 4);
    public bool isStack = false;
    public int costTosell = 0;
    public bool tool = false;
}
