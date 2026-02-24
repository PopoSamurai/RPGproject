using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TurnMovementSystem : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap highlightTilemap;

    [Header("Highlight Tile")]
    public Tile highlightTile;

    [Header("Active Unit")]
    public Transform activeUnit;

    [Header("Movement Points")]
    public int moveRange = 4;

    private List<Vector3Int> reachableTiles = new List<Vector3Int>();

    void Start()
    {
        ShowReachableTiles();
    }
    void Update()
    {
        HandleClickMovement();
    }
    // 1. Pokaz dostêpne pola ruchu
    void ShowReachableTiles()
    {
        highlightTilemap.ClearAllTiles();
        reachableTiles.Clear();

        Vector3Int unitCell =
            groundTilemap.WorldToCell(activeUnit.position);

        for (int x = -moveRange; x <= moveRange; x++)
        {
            for (int y = -moveRange; y <= moveRange; y++)
            {
                int distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (distance <= moveRange)
                {
                    Vector3Int tilePos =
                        new Vector3Int(unitCell.x + x, unitCell.y + y, 0);

                    if (groundTilemap.HasTile(tilePos))
                    {
                        reachableTiles.Add(tilePos);
                        highlightTilemap.SetTile(tilePos, highlightTile);
                    }
                }
            }
        }
    }
    // 2. Kliknij tile -> przesuñ unit
    void HandleClickMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3Int clickedCell =
                groundTilemap.WorldToCell(mouseWorld);

            if (reachableTiles.Contains(clickedCell))
            {
                MoveUnit(clickedCell);
            }
        }
    }
    // 3. Przesuwanie postaci
    void MoveUnit(Vector3Int targetCell)
    {
        Vector3 targetWorld =
            groundTilemap.GetCellCenterWorld(targetCell);

        activeUnit.position = targetWorld;
        ShowReachableTiles();
    }
}