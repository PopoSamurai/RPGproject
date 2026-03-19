using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardSystem : MonoBehaviour
{
    public static BoardSystem Instance;
    public LayerMask boardMask;

    public List<BoardLane> playerLines;
    public List<BoardLane> enemyLines;

    void Awake()
    {
        Instance = this;
    }
    public GameObject GetPointUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, boardMask))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
    public Unit GetFrontTarget(BoardSlot attackerSlot)
    {
        var enemyLinesList = attackerSlot.owner == SlotOwner.Player
            ? enemyLines
            : playerLines;

        var enemyLine = enemyLinesList
            .FirstOrDefault(l => l == attackerSlot.line);

        if (enemyLine == null) return null;

        return enemyLine.GetFrontUnit();
    }
}