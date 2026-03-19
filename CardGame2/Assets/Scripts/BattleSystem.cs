using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ExecuteTurn()
    {
        StartCoroutine(TurnRoutine());
    }
    void TickAllUnits()
    {
        var allUnits = FindObjectsOfType<Unit>();

        foreach (var unit in allUnits)
        {
            unit.Tick();
        }
    }
    IEnumerator ResolveAttacks()
    {
        foreach (var line in BoardSystem.Instance.playerLines)
        {
            foreach (var slot in line.slots)
            {
                if (!slot.occupied) continue;

                var unit = slot.GetComponentInChildren<Unit>();

                if (unit != null && unit.IsReady)
                {
                    unit.PerformAttack();
                    unit.counter = unit.baseCounter;

                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
        foreach (var line in BoardSystem.Instance.enemyLines)
        {
            foreach (var slot in line.slots)
            {
                if (!slot.occupied) continue;

                var unit = slot.GetComponentInChildren<Unit>();

                if (unit != null && unit.IsReady)
                {
                    unit.PerformAttack();
                    unit.counter = unit.baseCounter;

                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }
    void CleanupBoard()
    {
        foreach (var line in BoardSystem.Instance.playerLines)
            line.Collapse();

        foreach (var line in BoardSystem.Instance.enemyLines)
            line.Collapse();
    }
    public IEnumerator TurnRoutine()
    {
        // Tick counterów
        TickAllUnits();

        yield return new WaitForSeconds(0.3f);

        // Ataki
        yield return StartCoroutine(ResolveAttacks());

        yield return new WaitForSeconds(0.3f);

        // Cleanup (œmierci + przesuniêcia)
        CleanupBoard();
    }
}