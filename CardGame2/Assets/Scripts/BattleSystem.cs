using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public CardData unit;
    public int turn;
    public int laneIndex;
    public bool isLeader;
}
public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;
    public CardData playerLeaderData;
    public BoardSlot playerLeaderSlot;

    public GameObject playerCardPrefab;
    public GameObject enemyCardPrefab;

    public List<EnemySpawn> enemySpawns;
    private int currentTurn = 0;
    public GameObject hitEffectPrefab;
    public Vector3 LastAttackerPosition;
    void Awake()
    {
        Instance = this;
        StartBattle();
    }
    public void StartBattle()
    {
        SpawnPlayerLeader();
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
    IEnumerator ResolveCombatLoop()
    {
        Debug.Log("[COMBAT] START");

        while (true)
        {
            var readyUnits = FindObjectsOfType<Unit>()
                .Where(u => u.IsReady && !u.isDead && u.CurrentSlot != null)
                .OrderBy(u => u.CurrentSlot.line.laneIndex)
                .ThenBy(u => u.CurrentSlot.indexInLine)
                .ToList();

            if (readyUnits.Count == 0)
                break;

            foreach (var unit in readyUnits)
            {
                Debug.Log($"[COMBAT] {unit.name} attacks");
                yield return StartCoroutine(unit.PerformAttackRoutine());
                yield return new WaitUntil(() =>
                {
                    return FindObjectsOfType<Unit>().All(u => !u.IsAnimating);
                });

                unit.counter = unit.baseCounter;
                unit.IsReady = false;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    public void OnLeaderDied(Unit leader)
    {
        if (leader.owner == SlotOwner.Player)
        {
            Debug.Log("PLAYER LOSES");
        }
        else
        {
            Debug.Log("PLAYER WINS");
        }
    }
    public IEnumerator TurnRoutine()
    {
        currentTurn++;
        SpawnEnemiesForTurn();
        TickAllUnits();

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(ResolveCombatLoop());
    }
    void SpawnEnemiesForTurn()
    {
        foreach (var spawn in enemySpawns)
        {
            if (spawn.turn == currentTurn)
            {
                var lane = BoardSystem.Instance.enemyLines[spawn.laneIndex];
                var slot = lane.GetFirstFreeSlot();

                if (slot != null)
                {
                    SpawnEnemy(spawn, slot);
                }
            }
        }
    }
    void SpawnEnemy(EnemySpawn spawn, BoardSlot slot)
    {
        var go = Instantiate(enemyCardPrefab);

        var unit = go.GetComponentInChildren<Unit>();
        var view = go.GetComponentInChildren<CardView>();

        view.Init(spawn.unit);
        unit.Init(spawn.unit);
        view.BindUnit(unit);

        unit.owner = SlotOwner.Enemy;
        unit.isLeader = spawn.isLeader;
        CardExecutor.Instance.TryPlayUnitCard(view, slot, true, true);

        slot.line?.Collapse();
    }
    void SpawnPlayerLeader()
    {
        SpawnLeader(playerLeaderData, playerLeaderSlot, SlotOwner.Player);
        playerLeaderSlot.line?.Collapse();
    }
    void SpawnLeader(CardData data, BoardSlot slot, SlotOwner owner)
    {
        GameObject prefab = owner == SlotOwner.Player
            ? playerCardPrefab
            : enemyCardPrefab;

        if (prefab == null)
        {
            Debug.LogError("Prefab jest NULL!");
            return;
        }
        if (slot == null)
        {
            Debug.LogError("Slot lidera jest NULL!");
            return;
        }
        if (data == null)
        {
            Debug.LogError("CardData lidera jest NULL!");
            return;
        }
        var go = Instantiate(prefab);
        var unit = go.GetComponentInChildren<Unit>(true);
        var view = go.GetComponentInChildren<CardView>();
        Debug.Log($"SpawnLeader prefab name: {prefab?.name}");
        if (unit == null)
        {
            Debug.LogError($"Prefab {prefab.name} NIE MA komponentu Unit!");
            Destroy(go);
            return;
        }
        if (view == null)
        {
            Debug.LogError($"Prefab {prefab.name} NIE MA komponentu CardView!");
            Destroy(go);
            return;
        }
        view.Init(data);
        unit.Init(data);
        view.BindUnit(unit);
        unit.isLeader = true;
        unit.owner = owner;
        CardExecutor.Instance.TryPlayUnitCard(view, slot, true, true);
        slot.line?.Collapse();
    }
    public IEnumerator ScreenShake(float duration = 0.1f, float strength = 10f)
    {
        Vector3 original = Camera.main.transform.position;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float x = Random.Range(-strength, strength);
            float y = Random.Range(-strength, strength);

            Camera.main.transform.position = original + new Vector3(x, y, 0);
            yield return null;
        }

        Camera.main.transform.position = original;
    }
}