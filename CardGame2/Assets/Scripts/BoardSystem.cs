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
        if (attackerSlot == null)
        {
            return null;
        }
        var enemyLine = GetOppositeLine(attackerSlot.line);

        if (enemyLine == null)
        {
            return null;
        }
        for (int i = 0; i < enemyLine.slots.Count; i++)
        {
            var slot = enemyLine.slots[i];

            if (slot.occupied)
            {
                var unit = slot.GetComponentInChildren<Unit>();

                if (unit != null)
                {
                    return unit;
                }
            }
        }
        return null;
    }
    public BoardLane GetOppositeLine(BoardLane line)
    {
        if (playerLines.Contains(line))
        {
            int index = playerLines.IndexOf(line);
            return enemyLines[index];
        }
        if (enemyLines.Contains(line))
        {
            int index = enemyLines.IndexOf(line);
            return playerLines[index];
        }
        return null;
    }
}