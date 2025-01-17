using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactiveMap;
    [SerializeField] private Tile hiddenInteractiveTile;
    [SerializeField] private Tile interactedTile;
    void Start()
    {
        foreach(var position in interactiveMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactiveMap.GetTile(position);
            if(tile != null && tile.name == "interactive")
                interactiveMap.SetTile(position, hiddenInteractiveTile);
        }
    }
    public bool isInteractive(Vector3Int position)
    {
        TileBase tile = interactiveMap.GetTile(position);

        if(tile != null)
        {
            if(tile.name == "interactive")
            {
                return true;
            }
        }

        return false;
    }
    public void SetInteracted(Vector3Int position)
    {
        interactiveMap.SetTile(position, interactedTile);
    }
}
