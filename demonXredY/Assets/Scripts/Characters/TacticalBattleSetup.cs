using System.Collections.Generic;
using UnityEngine;

// Spawn before TacticalTurnManager.Start().
[DefaultExecutionOrder(-200)]
public class TacticalBattleSetup : MonoBehaviour
{
    [System.Serializable]
    public class TeamEntry
    {
        public TacticalUnitData unitData;
        [Min(1)] public int count = 1;
    }

    [Header("Prefab with TacticalUnit + Collider + sprite")]
    public TacticalUnit unitPrefab;
    [Header("Teams - ScriptableObjects")]
    public List<TeamEntry> playerTeam = new List<TeamEntry>();
    public List<TeamEntry> enemyTeam = new List<TeamEntry>();
    [Header("Spawn anchors (world positions on the map)")]
    public Transform playerSpawnCenter;
    public Transform enemySpawnCenter;
    [Header("Grid and turn manager")]
    public TacticalGridMovement grid;
    public TacticalTurnManager turnManager;
    [Min(0)] public float unitGroundOffset = 0f;
    [Header("Optional parents")]
    public Transform playerParent;
    public Transform enemyParent;

    private readonly HashSet<Vector2Int> reserved = new HashSet<Vector2Int>();
    private readonly List<TacticalUnit> spawned = new List<TacticalUnit>();

    private void Awake()
    {
        if (unitPrefab == null || grid == null || turnManager == null || playerSpawnCenter == null || enemySpawnCenter == null)
        {
            Debug.LogError("TacticalBattleSetup: assign Unit Prefab, Grid, Turn Manager and both Spawn Centers.", this);
            return;
        }
        // Scene units can be included, but the default is a battle built from the two team lists.
        turnManager.units.Clear();
        SpawnTeam(playerTeam, true, playerSpawnCenter.position, playerParent);
        SpawnTeam(enemyTeam, false, enemySpawnCenter.position, enemyParent);
        turnManager.units.AddRange(spawned);
    }

    private void SpawnTeam(List<TeamEntry> entries, bool playerSide, Vector3 anchor, Transform parent)
    {
        foreach (TeamEntry entry in entries)
        {
            if (entry == null || entry.unitData == null) continue;
            for (int i = 0; i < entry.count; i++)
            {
                Vector3 position;
                Vector2Int coord;
                if (!FindFreeCell(anchor, out coord, out position))
                {
                    Debug.LogWarning("No free walkable cell for " + entry.unitData.displayName);
                    continue;
                }
                TacticalUnit unit = Instantiate(unitPrefab, position, Quaternion.identity, parent);
                unit.name = (playerSide ? "Player_" : "Enemy_") + entry.unitData.displayName + "_" + (i + 1);
                unit.Initialize(entry.unitData, playerSide);
                // Align the instantiated unit's real collider bottom to the terrain surface.
                // Prefab collider bounds are unreliable before instantiation.
                float bottom = float.PositiveInfinity;
                foreach (Collider body in unit.GetComponentsInChildren<Collider>())
                {
                    if (body != null && body.enabled && !body.isTrigger)
                        bottom = Mathf.Min(bottom, body.bounds.min.y);
                }
                if (!float.IsPositiveInfinity(bottom))
                    unit.transform.position += Vector3.up * (position.y + unitGroundOffset - bottom);
                else
                    Debug.LogWarning("Spawned unit has no enabled non-trigger Collider: " + unit.name, unit);

                // Both teams face one another on spawn, regardless of world axis.
                TacticalUnitSprite appearance = unit.GetComponent<TacticalUnitSprite>();
                if (appearance != null)
                {
                    Transform oppositeSpawn = playerSide ? enemySpawnCenter : playerSpawnCenter;
                    appearance.FaceTarget(oppositeSpawn.position);
                }
                reserved.Add(coord);
                spawned.Add(unit);
            }
        }
    }

    private bool FindFreeCell(Vector3 anchor, out Vector2Int selected, out Vector3 world)
    {
        selected = Vector2Int.zero;
        world = Vector3.zero;
        float size = grid.cellSize;
        if (size <= 0f) return false;
        int centerX = Mathf.RoundToInt((anchor.x - grid.origin.x) / size + (grid.gridWidth - 1) / 2f);
        int centerZ = Mathf.RoundToInt((anchor.z - grid.origin.z) / size + (grid.gridDepth - 1) / 2f);
        int maxRadius = grid.gridWidth + grid.gridDepth;
        int walkable = LayerMask.NameToLayer(grid.walkableLayerName);
        for (int radius = 0; radius <= maxRadius; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            for (int dz = -radius; dz <= radius; dz++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dz) != radius) continue;
                int x = centerX + dx, z = centerZ + dz;
                if (x < 0 || z < 0 || x >= grid.gridWidth || z >= grid.gridDepth) continue;
                Vector2Int coord = new Vector2Int(x, z);
                if (reserved.Contains(coord)) continue;
                float wx = grid.origin.x + (x - (grid.gridWidth - 1) / 2f) * size;
                float wz = grid.origin.z + (z - (grid.gridDepth - 1) / 2f) * size;
                RaycastHit[] hits = Physics.RaycastAll(new Vector3(wx, grid.raycastStartY, wz), Vector3.down, 1000f, grid.terrainMask, QueryTriggerInteraction.Ignore);
                System.Array.Sort(hits, (a,b) => b.point.y.CompareTo(a.point.y));
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.layer != walkable || hit.normal.y < 0.5f) continue;
                    Vector3 probe = new Vector3(wx, hit.point.y + grid.blockingCheckHeight, wz);
                    Collider[] obstacles = Physics.OverlapBox(probe, new Vector3(size * 0.3f, grid.blockingCheckHeight, size * 0.3f), Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
                    bool blocked = false;
                    foreach (Collider obstacle in obstacles)
                    {
                        if (obstacle.GetComponentInParent<TacticalUnit>() != null || (grid.blockingMask.value & (1 << obstacle.gameObject.layer)) != 0)
                        { blocked = true; break; }
                    }
                    if (blocked) continue;
                    selected = coord;
                    world = new Vector3(wx, hit.point.y, wz);
                    return true;
                }
            }
        }
        return false;
    }
}
