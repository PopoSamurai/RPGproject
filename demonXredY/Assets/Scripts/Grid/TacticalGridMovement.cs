using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TacticalGridMovement : MonoBehaviour
{
    [Header("Grid")]
    private Transform actingUnit;
    public Camera gameCamera;
    public Vector3 origin = Vector3.zero;
    public float cellSize = 1f;
    public int gridWidth = 20;
    public int gridDepth = 20;
    public float raycastStartY = 100f;

    [Header("Terrain layers")]
    public LayerMask terrainMask;
    public string walkableLayerName = "Walkable";

    [Header("Blocking")]
    public LayerMask blockingMask;
    public float blockingCheckHeight = 0.5f;

    [Header("Movement")]
    private int movementRange = 5;
    public float maxStepHeight = 1.1f;
    private float moveSpeed = 4f;

    [Header("Jump action")]
    [Min(1)] public int maxJumpGapCells = 1;
    public float maxJumpHeight = 1.5f;
    public float jumpArcHeight = 1.25f;

    [Header("Pickup / Throw")]
    public int throwRange = 3;
    [Min(1)] private int carryStrength = 5;
    public GameObject pickupTilePrefab;
    public float throwArcHeight = 2f;
    [Tooltip("Legacy setting (not used for character carrying). Use heldFeetHeightFraction below.")]
    public float carryHeight = 1.5f;
    [Header("Held unit position")]
    [Range(0.35f, 1.25f)] public float heldFeetHeightFraction = 0.78f;
    [Tooltip("Fine-tune held sprite height in world units; negative = lower.")]
    public float heldVerticalOffset = -0.05f;

    private IEnumerator FinishPickupPose(TacticalUnitSprite sprite)
    {
        if (sprite == null) yield break;
        yield return new WaitForSeconds(sprite.pickupAnimationDuration);
        // A throw may already have started; do not override Throw animation.
        if (isCarrying && !isMoving && sprite != null)
            sprite.HoldPickupPose();
    }
    [Header("Throw landing VFX")]
    [Tooltip("Prefab ParticleSystem with Play On Awake enabled; spawned at ground contact.")]
    public GameObject throwLandingDustPrefab;
    [Min(0f)] public float dustHeightOffset = 0.03f;

    private void SpawnLandingDust(Vector3 landingPosition, float surfaceY)
    {
        if (throwLandingDustPrefab == null) return;
        landingPosition.y = surfaceY + dustHeightOffset;
        GameObject fx = Instantiate(throwLandingDustPrefab, landingPosition, Quaternion.identity);
        ParticleSystem[] particles = fx.GetComponentsInChildren<ParticleSystem>();
        float lifetime = 3f;
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
            var main = ps.main;
            lifetime = Mathf.Max(lifetime, main.duration + main.startLifetime.constantMax + 0.5f);
        }
        Destroy(fx, lifetime);
    }

    private static TacticalUnitSprite GetUnitSprite(Transform root)
    {
        return root != null ? root.GetComponent<TacticalUnitSprite>() : null;
    }

    [Header("Attack")]
    private bool isArcher = false;
    public string enemyTag = "enemy";
    [Min(1)] private int archerAttackRange = 4;
    [Min(1)] private int attackDamage = 1;

    public GameObject attackTilePrefab;
    public GameObject movementTilePrefab;
    public float highlightYOffset = 0.04f;

    private readonly List<GameObject> movementTiles = new List<GameObject>();
    private readonly HashSet<Vector2Int> reachableCells = new HashSet<Vector2Int>();
    private readonly HashSet<Vector2Int> jumpCells = new HashSet<Vector2Int>();
    private readonly HashSet<Vector2Int> throwCells = new HashSet<Vector2Int>();
    private readonly Dictionary<Vector2Int, Transform> throwEnemyTargets = new Dictionary<Vector2Int, Transform>();
    private readonly Dictionary<Vector2Int, Transform> throwFriendlyTargets = new Dictionary<Vector2Int, Transform>();
    private readonly HashSet<Vector2Int> attackCells = new HashSet<Vector2Int>();
    private readonly HashSet<Vector2Int> pickupCells = new HashSet<Vector2Int>();
    private readonly Dictionary<Vector2Int, Transform> attackTargets = new Dictionary<Vector2Int, Transform>();
    private readonly Dictionary<Vector2Int, Transform> pickupTargets = new Dictionary<Vector2Int, Transform>();
    private readonly Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();

    public TacticalTurnManager turnManager;
    public bool IsBusy => isMoving || isCarrying;
    private TacticalUnit CurrentUnit => actingUnit == null ? null : actingUnit.GetComponent<TacticalUnit>();
    private bool PlayerCanControl => turnManager != null && turnManager.ActiveUnit != null && turnManager.ActiveUnit.isPlayerControlled && turnManager.ActiveUnit.IsActiveUnit && CurrentUnit == turnManager.ActiveUnit;

    public void SetActiveUnit(TacticalUnit unit)
    {
        DeselectUnit();
        actingUnit = unit != null && unit.isPlayerControlled ? unit.transform : null;
        if (actingUnit == null) return;
        movementRange = unit.movementRange;
        moveSpeed = unit.moveSpeed;
        carryStrength = unit.strength;
        attackDamage = unit.attackDamage;
        isArcher = unit.isArcher;
        archerAttackRange = unit.attackRange;
        SelectUnit(unit);
    }

    private bool isSelected;
    private bool isJumpMode;
    private bool isMoving;
    private bool isCarrying;
    private bool isAttackMode;
    private bool isPickupMode;
    private Transform carriedObject;
    private float carriedBottomOffset = 0.5f;
    private int walkableLayer;

    [Header("Camera")]
    public TacticalCameraController tacticalCamera;

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };
    private class Cell
    {
        public Vector2Int coord;
        public float surfaceY;
        public Cell(Vector2Int coord, float surfaceY)
        {
            this.coord = coord;
            this.surfaceY = surfaceY;
        }
    }
    private void SelectUnit(Transform unit)
    {
        if (unit == null) return;

        actingUnit = unit;
        isSelected = true;

        isJumpMode = false;
        isAttackMode = false;
        isPickupMode = false;

        if (tacticalCamera != null)
            tacticalCamera.SetTarget(unit);

        RefreshMovementTiles();
    }
    private void Start()
    {
        if (gameCamera == null) gameCamera = Camera.main;
        walkableLayer = LayerMask.NameToLayer(walkableLayerName);

        if (walkableLayer == -1)
        {
            Debug.LogError($"Nie znaleziono warstwy '{walkableLayerName}'!");
            enabled = false;
            return;
        }
        if (cellSize <= 0f || gridWidth <= 0 || gridDepth <= 0)
        {
            Debug.LogError("Nieprawidłowe ustawienia siatki.");
            enabled = false;
            return;
        }

        BuildGrid();
        ClearMovementTiles(); // Na początku gry zasięg jest ukryty.
    }

    private void Update()
    {
        if (isMoving || actingUnit == null || gameCamera == null || !PlayerCanControl) return;

        if (Input.GetMouseButtonDown(0)) HandleClick();
        if (isSelected && Input.GetKeyDown(KeyCode.Escape)) DeselectUnit();
    }

    // Podlacz te metody do UI Button -> On Click().
    public void OnAttackButton()
    {
        if (!PlayerCanControl || isMoving || isCarrying || CurrentUnit.hasActed) return;
        isSelected = true;
        isAttackMode = !isAttackMode;
        isPickupMode = false;
        isJumpMode = false;
        RefreshMovementTiles();
    }

    // Pierwsze klikniecie: wybierz obiekt do podniesienia.
    // Po podniesieniu: kliknij pole lub jednostke, w ktora rzucasz.
    public void OnThrowButton()
    {
        if (!PlayerCanControl || isMoving || CurrentUnit.hasActed) return;
        isSelected = true;
        isAttackMode = false;
        isJumpMode = false;
        if (!isCarrying) { isPickupMode = !isPickupMode; RefreshMovementTiles(); }
        else RefreshMovementTiles();
    }

    public void OnMoveButton()
    {
        if (!PlayerCanControl || isMoving || isCarrying) return;
        isSelected = true;
        isAttackMode = false;
        isPickupMode = false;
        isJumpMode = false;
        RefreshMovementTiles();
    }

    public void OnJumpButton()
    {
        if (!PlayerCanControl || isMoving || isCarrying || CurrentUnit.RemainingMovement <= 0) return;
        isSelected = true;
        isAttackMode = false;
        isPickupMode = false;
        isJumpMode = !isJumpMode;
        RefreshMovementTiles();
    }

    private void HandleClick()
    {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, terrainMask, QueryTriggerInteraction.Ignore))
            return;
        if (hit.normal.y < 0.5f) return;

        Vector2Int clickedCoord = WorldToGrid(hit.point);
        Vector2Int playerCoord = WorldToGrid(actingUnit.position);

        if (!isCarrying &&
            !isAttackMode &&
            !isPickupMode &&
            !isJumpMode)
        {
            TacticalUnit clickedUnit = FindUnitAtCell(clickedCoord);

            if (clickedUnit != null && clickedUnit == turnManager.ActiveUnit)
            {
                SelectUnit(clickedUnit);
                return;
            }
        }
        if (clickedCoord == playerCoord && !isCarrying && !isAttackMode && !isPickupMode)
        {
            SelectUnit(actingUnit);
            return;
        }

        if (!isSelected || !PlayerCanControl) return;

        if (isPickupMode)
        {
            if (CurrentUnit.hasActed) return;
            if (!pickupTargets.TryGetValue(clickedCoord, out Transform selectedObject) || selectedObject == null)
                return;
            if (!CanCarry(selectedObject))
            {
                Debug.Log("Ten obiekt jest zbyt ciężki, by go podnieść.");
                return;
            }

            ClearMovementTiles();
            isPickupMode = false;
            PickUpObject(selectedObject);
            return;
        }

        if (isAttackMode)
        {
            if (CurrentUnit.hasActed) return;
            if (!attackTargets.TryGetValue(clickedCoord, out Transform enemy) || enemy == null) return;
            if (!IsEnemyInAttackRange(enemy)) return;
            ClearMovementTiles();
            isSelected = false;
            isAttackMode = false;
            PerformAttack(enemy);
            return;
        }

        if (isCarrying)
        {
            if (CurrentUnit.hasActed) return;
            if (!throwCells.Contains(clickedCoord))
                return;

            // Kliknięto przeciwnika.
            if (throwEnemyTargets.TryGetValue(
                clickedCoord, out Transform enemy) && enemy != null)
            {
                if (!TryGetThrowLandingCell(
                    WorldToGrid(actingUnit.position),
                    WorldToGrid(enemy.position),
                    out Cell landingCell))
                {
                    Debug.Log(
                        "Nie można rzucić istotą. " +
                        "Brak wolnego pola przed przeciwnikiem.");
                    return;
                }

                ClearMovementTiles();
                isSelected = false;

                StartCoroutine(
                    ThrowAtEnemy(enemy, landingCell));

                return;
            }
            // Kliknięto własną jednostkę.
            if (throwFriendlyTargets.TryGetValue(
                clickedCoord, out Transform friendly) &&
                friendly != null)
            {
                if (!TryGetThrowLandingCell(
                    WorldToGrid(actingUnit.position),
                    WorldToGrid(friendly.position),
                    out Cell landingCell))
                {
                    Debug.Log(
                        "Nie można rzucić jednostką. " +
                        "Brak wolnego pola przed celem.");
                    return;
                }

                ClearMovementTiles();
                isSelected = false;

                StartCoroutine(
                    ThrowAtEnemy(friendly, landingCell));

                return;
            }
            // Kliknięto zwykłe, wolne pole.
            if (IsCellBlocked(clickedCoord))
                return;

            if (!cells.TryGetValue(clickedCoord, out Cell throwTarget))
                return;

            ClearMovementTiles();
            isSelected = false;

            StartCoroutine(ThrowUnitToCell(throwTarget));
            return;
        }

        if (isJumpMode)
        {
            if (!jumpCells.Contains(clickedCoord)) return;
            if (IsCellBlocked(clickedCoord)) return;
            if (!cells.TryGetValue(clickedCoord, out Cell jumpTarget)) return;

            ClearMovementTiles();
            isSelected = false;
            isJumpMode = false;
            StartCoroutine(JumpToCell(jumpTarget));
            return;
        }

        if (CurrentUnit.RemainingMovement <= 0 || !reachableCells.Contains(clickedCoord) || IsCellBlocked(clickedCoord)) return;
        List<Cell> path = FindWalkPath(playerCoord, clickedCoord);
        if (path == null || path.Count == 0) return;

        ClearMovementTiles();
        isSelected = false;
        StartCoroutine(FollowPath(path));
    }
    private bool TryGetThrowLandingCell(
    Vector2Int playerCoord,
    Vector2Int enemyCoord,
    out Cell landingCell)
    {
        landingCell = null;
        Vector2Int direction = enemyCoord - playerCoord;

        if (direction.x != 0 && direction.y != 0)
            return false;

        if (direction == Vector2Int.zero)
            return false;

        direction = new Vector2Int(
            direction.x == 0 ? 0 : (int)Mathf.Sign(direction.x),
            direction.y == 0 ? 0 : (int)Mathf.Sign(direction.y));

        Vector2Int landingCoord = enemyCoord - direction;

        if (landingCoord == playerCoord)
            return false;

        if (!cells.TryGetValue(landingCoord, out Cell cell))
            return false;

        if (IsCellBlocked(landingCoord))
            return false;

        landingCell = cell;
        return true;
    }
    private IEnumerator ThrowAtEnemy(
    Transform enemy,
    Cell landingCell)
    {
        if (enemy != null) FaceActingUnit(enemy.position);
        isMoving = true;

        if (carriedObject == null || enemy == null)
        {
            isMoving = false;
            yield break;
        }

        TacticalUnitSprite throwerSprite = GetUnitSprite(actingUnit);
        if (throwerSprite != null)
        {
            throwerSprite.FaceTarget(enemy.position);
            throwerSprite.PlayThrow();
        }
        TacticalUnitSprite thrownSprite = GetUnitSprite(carriedObject);
        // Keep the thrown character in Held pose throughout the entire flight.
        Transform thrownObject = carriedObject;

        Vector3 startPosition = thrownObject.position;
        Vector3 targetPosition = GridToWorld(landingCell.coord);

        Collider[] carriedColliders =
            thrownObject.GetComponentsInChildren<Collider>();

        targetPosition.y = landingCell.surfaceY + carriedBottomOffset;

        if (thrownSprite != null) thrownSprite.FaceDirection(targetPosition - startPosition);
        thrownObject.SetParent(null, true);
        FaceThrownUnit(thrownObject, startPosition, targetPosition);

        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = Mathf.Max(distance / moveSpeed, 0.25f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Jeśli przeciwnik zniknie podczas animacji, nie przerywamy lotu.
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t);

            position.y += Mathf.Sin(t * Mathf.PI) * throwArcHeight;

            thrownObject.position = position;

            yield return null;
        }

        thrownObject.position = targetPosition;
        if (thrownSprite != null) thrownSprite.SetHeld(false);
        SpawnLandingDust(targetPosition, landingCell.surfaceY);
        FaceThrownUnit(thrownObject, startPosition, targetPosition);

        foreach (Collider col in carriedColliders)
        {
            if (col != null)
                col.enabled = true;
        }

        Rigidbody carriedBody = thrownObject.GetComponent<Rigidbody>();

        if (carriedBody != null)
            carriedBody.isKinematic = false;

        // Trafiona jednostka obraca się w stronę rzucającego.
        if (enemy != null)
        {
            FaceUnitTowards(enemy, actingUnit != null ? actingUnit.position : startPosition);
            enemy.gameObject.SendMessage(
                "ApplyDamage",
                attackDamage,
                SendMessageOptions.DontRequireReceiver);

            Debug.Log(
                $"Rzut istotą trafił {enemy.name}. " +
                $"Obrażenia: {attackDamage}.");
        }

        isCarrying = false;
        isMoving = false;
        if (CurrentUnit != null) CurrentUnit.hasActed = true;
        carriedObject = null;

        ClearMovementTiles();

        isSelected = false;
        isJumpMode = false;
        isAttackMode = false;
        isPickupMode = false;

        Debug.Log("Rzut na przeciwnika zakończony.");
    }
    private void RefreshMovementTiles()
    {
        ClearMovementTiles();
        if (actingUnit == null || movementTilePrefab == null || !PlayerCanControl) return;

        Vector2Int start = WorldToGrid(actingUnit.position);
        if (!cells.ContainsKey(start)) return;

        if (isCarrying) BuildThrowTargets(start);
        else if (isPickupMode) BuildPickupTargets(start);
        else if (isAttackMode && !CurrentUnit.hasActed) BuildAttackTargets(start);
        else if (isJumpMode && CurrentUnit.RemainingMovement > 0) BuildJumpTargets(start);
        else if (CurrentUnit.RemainingMovement > 0) BuildWalkTargets(start);

        if (isPickupMode)
        {
            foreach (Vector2Int coord in pickupCells)
            {
                if (!cells.TryGetValue(coord, out Cell cell)) continue;
                GameObject prefab = pickupTilePrefab != null ? pickupTilePrefab : movementTilePrefab;
                Vector3 position = GridToWorld(coord);
                position.y = cell.surfaceY + highlightYOffset;
                GameObject tile = Instantiate(prefab, position, Quaternion.Euler(90f, 0f, 0f));
                tile.transform.localScale = new Vector3(cellSize * 0.95f, cellSize * 0.95f, 1f);
                if (pickupTilePrefab == null) TintTile(tile, new Color(1f, 0.75f, 0.1f, 1f));
                movementTiles.Add(tile);
            }
            return;
        }

        if (isAttackMode)
        {
            foreach (Vector2Int coord in attackCells)
            {
                if (!cells.TryGetValue(coord, out Cell cell)) continue;
                GameObject tilePrefab = attackTilePrefab != null ? attackTilePrefab : movementTilePrefab;
                if (tilePrefab == null) continue;
                Vector3 position = GridToWorld(coord);
                position.y = cell.surfaceY + highlightYOffset;
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.Euler(90f, 0f, 0f));
                tile.transform.localScale = new Vector3(cellSize * 0.95f, cellSize * 0.95f, 1f);
                if (attackTilePrefab == null) TintTileRed(tile);
                movementTiles.Add(tile);
            }
            return;
        }

        HashSet<Vector2Int> toShow = isCarrying ? throwCells : (isJumpMode ? jumpCells : reachableCells);
        foreach (Vector2Int coord in toShow)
        {
            if (!cells.TryGetValue(coord, out Cell cell)) continue;
            Vector3 position = GridToWorld(coord);
            position.y = cell.surfaceY + highlightYOffset;
            GameObject tile = Instantiate(movementTilePrefab, position, Quaternion.Euler(90f, 0f, 0f));
            tile.transform.localScale = new Vector3(cellSize * 0.95f, cellSize * 0.95f, 1f);
            movementTiles.Add(tile);
        }
    }

    private void BuildAttackTargets(Vector2Int start)
    {
        attackCells.Clear();
        attackTargets.Clear();
        pickupCells.Clear();
        pickupTargets.Clear();
        foreach (TacticalUnit target in FindObjectsOfType<TacticalUnit>())
        {
            if (!target.IsAlive || target.isPlayerControlled == CurrentUnit.isPlayerControlled) continue;
            Vector2Int coord = WorldToGrid(target.transform.position);
            int distance = Mathf.Abs(coord.x - start.x) + Mathf.Abs(coord.y - start.y);
            if (distance < 1 || distance > (isArcher ? archerAttackRange : 1) || !cells.ContainsKey(coord)) continue;
            attackCells.Add(coord);
            attackTargets[coord] = target.transform;
        }
    }

    private bool IsEnemyInAttackRange(Transform enemy)
    {
        if (enemy == null) return false;
        Vector2Int playerCoord = WorldToGrid(actingUnit.position);
        Vector2Int enemyCoord = WorldToGrid(enemy.position);
        int distance = Mathf.Abs(enemyCoord.x - playerCoord.x) + Mathf.Abs(enemyCoord.y - playerCoord.y);
        return distance >= 1 && distance <= (isArcher ? archerAttackRange : 1);
    }

    // Kierunek wizualny niezalezny od obrotu kamery i kapsulki.
    private void FaceActingUnit(Vector3 targetPosition)
    {
        if (actingUnit == null) return;
        TacticalUnitSprite sprite = actingUnit.GetComponent<TacticalUnitSprite>();
        if (sprite != null) sprite.FaceTarget(targetPosition);
    }

    // Ustawia kierunek dowolnej jednostki, nie tylko aktywnej.
    private static void FaceUnitTowards(Transform unitTransform, Vector3 worldPosition)
    {
        if (unitTransform == null) return;
        TacticalUnitSprite sprite = unitTransform.GetComponent<TacticalUnitSprite>();
        if (sprite != null) sprite.FaceTarget(worldPosition);
    }

    // Zachowuje kierunek lotu po zakończeniu rzutu.
    private static void FaceThrownUnit(Transform thrown, Vector3 from, Vector3 to)
    {
        if (thrown == null) return;
        TacticalUnitSprite sprite = thrown.GetComponent<TacticalUnitSprite>();
        if (sprite != null) sprite.FaceDirection(to - from);
    }

    private static bool IsStraightThrow(Vector2Int from, Vector2Int to)
    {
        return (from.x == to.x) != (from.y == to.y);
    }

    private void PerformAttack(Transform enemy)
    {
        if (enemy == null) return;
        FaceActingUnit(enemy.position);
        // Trafiona jednostka odwraca się do napastnika.
        FaceUnitTowards(enemy, actingUnit.position);
        int damage = attackDamage;
        // Enemy może mieć własny komponent z metodą public void ApplyDamage(int damage).
        enemy.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
        if (CurrentUnit != null) CurrentUnit.hasActed = true;
        Debug.Log($"{(isArcher ? "Strzał z łuku" : "Atak wręcz")}: {enemy.name}, obrażenia: {damage}.");
    }

    private void TintTileRed(GameObject tile)
    {
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            // material tworzy instancję materiału dla tego konkretnego podświetlenia.
            rend.material.color = Color.red;
        }
    }

    private void BuildWalkTargets(Vector2Int start)
    {
        reachableCells.Clear();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();
        queue.Enqueue(start);
        distance[start] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (!cells.ContainsKey(next) || distance.ContainsKey(next) || IsCellBlocked(next)) continue;
                float heightDifference = Mathf.Abs(cells[next].surfaceY - cells[current].surfaceY);
                if (heightDifference > maxStepHeight) continue;
                int newDistance = distance[current] + 1;
                if (newDistance > (CurrentUnit != null && CurrentUnit.isPlayerControlled ? CurrentUnit.RemainingMovement : movementRange)) continue;
                distance[next] = newDistance;
                queue.Enqueue(next);
            }
        }

        foreach (Vector2Int coord in distance.Keys)
            if (coord != start) reachableCells.Add(coord);
    }

    private void BuildJumpTargets(Vector2Int start)
    {
        jumpCells.Clear();
        if (!cells.TryGetValue(start, out Cell startCell)) return;

        // To jest osobna akcja: lądowanie musi być w linii prostej,
        // a pomiędzy startem i celem musi istnieć co najmniej jedno pole bez Walkable.
        foreach (Vector2Int dir in directions)
        {
            for (int gap = 1; gap <= maxJumpGapCells; gap++)
            {
                int landingDistance = gap + 1;
                if (landingDistance > (CurrentUnit != null && CurrentUnit.isPlayerControlled ? CurrentUnit.RemainingMovement : movementRange)) break;

                Vector2Int landing = start + dir * landingDistance;
                bool clearGap = true;
                for (int i = 1; i <= gap; i++)
                {
                    Vector2Int middle = start + dir * i;
                    // Skok jest przeznaczony do pokonywania pól bez Walkable,
                    // np. wody lub dziury. Nie przeskakujemy przez inne pola terenu.
                    if (cells.ContainsKey(middle))
                    {
                        clearGap = false;
                        break;
                    }
                    if (IsBlockingVolumeAt(middle))
                    {
                        clearGap = false;
                        break;
                    }
                }
                if (!clearGap || !cells.TryGetValue(landing, out Cell landingCell)) continue;
                if (IsCellBlocked(landing)) continue;
                if (Mathf.Abs(landingCell.surfaceY - startCell.surfaceY) > maxJumpHeight) continue;
                jumpCells.Add(landing);
            }
        }
    }

    private bool IsCellBlocked(Vector2Int coord)
    {
        if (!cells.TryGetValue(coord, out Cell cell)) return true;
        Vector3 center = GridToWorld(coord);
        center.y = cell.surfaceY + blockingCheckHeight;
        // Sprawdzamy wszystkie warstwy, bo przeciwnicy mogą nie mieć osobnej warstwy.
        // Pole blokuje obiekt z Blocking Mask LUB obiekt oznaczony tagiem Enemy.
        Collider[] obstacles = Physics.OverlapBox(
            center,
            new Vector3(cellSize * 0.35f, blockingCheckHeight, cellSize * 0.35f),
            Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);

        foreach (Collider obstacle in obstacles)
        {
            if (obstacle == null) continue;
            // Własne i wrogie jednostki blokują zajętą kratkę.
            TacticalUnit occupiedUnit =
                obstacle.GetComponentInParent<TacticalUnit>();

            if (occupiedUnit != null)
            {
                if (actingUnit != null &&
                    (occupiedUnit.transform == actingUnit ||
                     occupiedUnit.transform.IsChildOf(actingUnit)))
                    continue;

                if (isCarrying &&
                    carriedObject != null &&
                    (occupiedUnit.transform == carriedObject ||
                     occupiedUnit.transform.IsChildOf(carriedObject)))
                    continue;

                return true;
            }

            if (actingUnit != null && (obstacle.transform == actingUnit || obstacle.transform.IsChildOf(actingUnit)))
                continue;

            bool onBlockingLayer = (blockingMask.value & (1 << obstacle.gameObject.layer)) != 0;
            if (onBlockingLayer || IsEnemyCollider(obstacle)) return true;
        }
        return false;
    }

    private bool IsBlockingVolumeAt(Vector2Int coord)
    {
        Vector3 center = GridToWorld(coord);
        center.y = raycastStartY * 0.5f;
        Vector3 halfExtents = new Vector3(cellSize * 0.3f, raycastStartY * 0.5f, cellSize * 0.3f);
        Collider[] obstacles = Physics.OverlapBox(center, halfExtents, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider obstacle in obstacles)
        {
            if (obstacle == null) continue;
            if (actingUnit != null && (obstacle.transform == actingUnit || obstacle.transform.IsChildOf(actingUnit))) continue;

            bool onBlockingLayer = (blockingMask.value & (1 << obstacle.gameObject.layer)) != 0;
            if (onBlockingLayer || IsEnemyCollider(obstacle)) return true;
        }
        return false;
    }

    private bool IsEnemyCollider(Collider obstacle)
    {
        if (obstacle == null) return false;

        // Tag może znajdować się na obiekcie głównym, a Collider na obiekcie potomnym.
        Transform current = obstacle.transform;
        while (current != null)
        {
            try
            {
                if (current.CompareTag(enemyTag)) return true;
            }
            catch (UnityException)
            {
                // Jeśli tag nie istnieje, pomijamy sprawdzanie tagu.
                return false;
            }
            current = current.parent;
        }
        return false;
    }

    private void EnterPickupMode()
    {
        isPickupMode = true;
        isJumpMode = false;
        isAttackMode = false;
        RefreshMovementTiles();
        if (pickupCells.Count == 0)
            Debug.Log("Nie ma obok żadnych obiektów, które możesz podnieść.");
        else
            Debug.Log("Wybierz podświetlone pole pod jednostką lub skrzynią, którą chcesz podnieść.");
    }

    private void BuildPickupTargets(Vector2Int start)
    {
        pickupCells.Clear();
        pickupTargets.Clear();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int coord = start + direction;
            if (!cells.ContainsKey(coord)) continue;

            Collider[] hits = Physics.OverlapBox(
                new Vector3(GridToWorld(coord).x, cells[coord].surfaceY + blockingCheckHeight, GridToWorld(coord).z),
                new Vector3(cellSize * 0.4f, blockingCheckHeight + 0.5f, cellSize * 0.4f),
                Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);

            foreach (Collider hit in hits)
            {
                if (hit == null) continue;
                Transform candidate = FindCarryableRoot(hit.transform);
                if (candidate == null || candidate == actingUnit || candidate.IsChildOf(actingUnit)) continue;
                if (isCarrying && carriedObject == candidate) continue;
                if (!IsCarryableObject(candidate) || !CanCarry(candidate)) continue;

                pickupTargets[coord] = candidate;
                pickupCells.Add(coord);
                break;
            }
        }
    }

    private Transform FindCarryableRoot(Transform candidate)
    {
        Transform current = candidate;
        while (current != null && current != actingUnit)
        {
            if (IsCarryableObject(current)) return current;
            current = current.parent;
        }
        return null;
    }

    private bool IsCarryableObject(Transform candidate)
    {
        if (candidate == null || candidate == actingUnit || candidate.IsChildOf(actingUnit)) return false;

        // Skrzynki i obiekty na Blocking Mask można podnosić.
        if ((blockingMask.value & (1 << candidate.gameObject.layer)) != 0) return true;

        // Wrogowie mogą być podnoszeni po tagu enemy, także gdy tag jest na rodzicu.
        Transform current = candidate;
        while (current != null)
        {
            if (candidate.GetComponent<TacticalUnit>() != null)
                return true;
            try
            {
                if (current.CompareTag(enemyTag)) return true;
            }
            catch (UnityException) { break; }
            current = current.parent;
        }

        // Zgodność z wcześniej przypisanym Carryable Unit.
        return false;
    }

    private bool CanCarry(Transform candidate)
    {
        if (candidate == null)
            return false;

        TacticalUnit carryInfo =
            candidate.GetComponent<TacticalUnit>();

        if (carryInfo == null)
            carryInfo = candidate.GetComponentInChildren<TacticalUnit>();

        int weight = carryInfo != null ? carryInfo.weight : 1;

        return weight <= carryStrength;
    }

    private void PickUpObject(Transform objectToCarry)
    {
        if (objectToCarry == null) return;
        FaceActingUnit(objectToCarry.position);

        TacticalUnitSprite lifterSprite = GetUnitSprite(actingUnit);
        if (lifterSprite != null)
        {
            lifterSprite.FaceTarget(objectToCarry.position);
            lifterSprite.PlayPickup();
        }
        carriedObject = objectToCarry;
        Collider[] carriedColliders = carriedObject.GetComponentsInChildren<Collider>();
        carriedBottomOffset = 0.5f;
        foreach (Collider col in carriedColliders)
            carriedBottomOffset = Mathf.Max(carriedBottomOffset, carriedObject.position.y - col.bounds.min.y);

        isCarrying = true;
        TacticalUnitSprite heldSprite = GetUnitSprite(carriedObject);
        if (heldSprite != null) heldSprite.SetHeld(true);
        carriedObject.SetParent(actingUnit, true);
        // Position the HELD UNIT'S FEET near the lifter's raised hands.
        // Do not add the full carried capsule height on top of carryHeight.
        Collider lifterCollider = actingUnit.GetComponent<Collider>();
        float carrierFeet = lifterCollider != null ? lifterCollider.bounds.min.y : actingUnit.position.y;
        float carrierHeight = lifterCollider != null ? lifterCollider.bounds.size.y : 1.5f;
        float desiredFeetY = carrierFeet + carrierHeight * heldFeetHeightFraction + heldVerticalOffset;
        carriedObject.position = new Vector3(actingUnit.position.x,
            desiredFeetY + carriedBottomOffset, actingUnit.position.z);
        if (lifterSprite != null) StartCoroutine(FinishPickupPose(lifterSprite));
        foreach (Collider col in carriedColliders) col.enabled = false;

        Rigidbody carriedBody = carriedObject.GetComponent<Rigidbody>();
        if (carriedBody != null) carriedBody.isKinematic = true;

        isJumpMode = false;
        isPickupMode = false;
        RefreshMovementTiles();
        Debug.Log($"Podniesiono: {carriedObject.name}. Kliknij podświetlone pole, aby rzucić.");
    }
    private void BuildThrowTargets(Vector2Int start)
    {
        throwCells.Clear();
        throwEnemyTargets.Clear();
        throwFriendlyTargets.Clear();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector2Int coord = new Vector2Int(x, z);

                if (coord == start ||
                    !cells.ContainsKey(coord) ||
                    IsCellBlocked(coord))
                    continue;

                int distance =
                    Mathf.Abs(coord.x - start.x) +
                    Mathf.Abs(coord.y - start.y);

                if (distance <= throwRange && IsStraightThrow(start, coord))
                    throwCells.Add(coord);
            }
        }
        GameObject[] enemies;
        try
        {
            enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        }
        catch (UnityException)
        {
            Debug.LogError($"Nie istnieje tag '{enemyTag}'.");
            return;
        }
        foreach (GameObject enemyObject in enemies)
        {
            if (enemyObject == null || !enemyObject.activeInHierarchy)
                continue;

            Transform enemy = enemyObject.transform;
            Vector2Int enemyCoord = WorldToGrid(enemy.position);

            if (!cells.ContainsKey(enemyCoord))
                continue;

            int distance =
                Mathf.Abs(enemyCoord.x - start.x) +
                Mathf.Abs(enemyCoord.y - start.y);

            if (distance < 1 || distance > throwRange || !IsStraightThrow(start, enemyCoord))
                continue;

            throwCells.Add(enemyCoord);
            throwEnemyTargets[enemyCoord] = enemy;
        }
        // Wyszukiwanie pozostałych jednostek, w które można rzucić.
        Collider[] allColliders = Physics.OverlapBox(
            actingUnit.position,
            new Vector3(throwRange * cellSize, 3f, throwRange * cellSize),
            Quaternion.identity,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in allColliders)
        {
            if (hit == null)
                continue;

            Transform candidate = FindCarryableRoot(hit.transform);

            if (candidate == null)
                continue;

            if (candidate == actingUnit || candidate.IsChildOf(actingUnit))
                continue;

            if (candidate == carriedObject ||
                candidate.IsChildOf(carriedObject))
                continue;

            Vector2Int coord = WorldToGrid(candidate.position);

            if (!cells.ContainsKey(coord))
                continue;

            int distance =
                Mathf.Abs(coord.x - start.x) +
                Mathf.Abs(coord.y - start.y);

            if (distance < 1 || distance > throwRange || !IsStraightThrow(start, coord))
                continue;

            // Przeciwnicy są obsługiwani przez osobny słownik.
            if (throwEnemyTargets.ContainsKey(coord))
                continue;

            throwCells.Add(coord);
            throwFriendlyTargets[coord] = candidate;
        }
    }
    private IEnumerator ThrowUnitToCell(Cell targetCell)
    {
        FaceActingUnit(CellWorldPosition(targetCell));
        isMoving = true;
        if (carriedObject == null)
        {
            isCarrying = false;
            isMoving = false;
            yield break;
        }

        TacticalUnitSprite throwerSprite = GetUnitSprite(actingUnit);
        if (throwerSprite != null)
        {
            throwerSprite.FaceTarget(GridToWorld(targetCell.coord));
            throwerSprite.PlayThrow();
        }
        TacticalUnitSprite thrownSprite = GetUnitSprite(carriedObject);
        // Keep the thrown character in Held pose throughout the entire flight.
        Vector3 startPosition = carriedObject.position;
        Vector3 targetPosition = GridToWorld(targetCell.coord);
        Collider[] carriedColliders = carriedObject.GetComponentsInChildren<Collider>();
        targetPosition.y = targetCell.surfaceY + carriedBottomOffset;

        if (thrownSprite != null) thrownSprite.FaceDirection(targetPosition - startPosition);
        carriedObject.SetParent(null, true);
        FaceThrownUnit(carriedObject, startPosition, targetPosition);
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = Mathf.Max(distance / moveSpeed, 0.25f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);
            position.y += Mathf.Sin(t * Mathf.PI) * throwArcHeight;
            carriedObject.position = position;
            yield return null;
        }
        carriedObject.position = targetPosition;
        if (thrownSprite != null) thrownSprite.SetHeld(false);
        SpawnLandingDust(targetPosition, targetCell.surfaceY);
        FaceThrownUnit(carriedObject, startPosition, targetPosition);
        foreach (Collider col in carriedColliders) col.enabled = true;
        Rigidbody carriedBody = carriedObject.GetComponent<Rigidbody>();
        if (carriedBody != null) carriedBody.isKinematic = false;

        isCarrying = false;
        isMoving = false;
        if (CurrentUnit != null) CurrentUnit.hasActed = true;
        carriedObject = null;
        ClearMovementTiles();
        isSelected = false;
        isJumpMode = false;
        Debug.Log("Rzut zakończony.");
    }

    private void TintTile(GameObject tile, Color color)
    {
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
                renderer.material.color = color;
        }
    }
    private void SelectUnit(TacticalUnit unit)
    {
        if (unit == null || !unit.IsAlive || !unit.isPlayerControlled || !PlayerCanControl || unit != turnManager.ActiveUnit)
            return;

        if (isCarrying || isMoving)
            return;

        actingUnit = unit.transform;

        // Statystyki kontrolera pochodzą od zaznaczonej jednostki.
        movementRange = unit.movementRange;
        moveSpeed = unit.moveSpeed;
        carryStrength = unit.strength;
        attackDamage = unit.attackDamage;
        isArcher = unit.isArcher;
        archerAttackRange = unit.attackRange;

        isSelected = true;
        isJumpMode = false;
        isAttackMode = false;
        isPickupMode = false;

        RefreshMovementTiles();

        Debug.Log(
            $"Wybrano: {unit.name} | " +
            $"HP: {unit.CurrentHealth}/{unit.maxHealth} | " +
            $"Siła: {unit.strength} | " +
            $"Atak: {unit.attackDamage} | " +
            $"Ruch: {unit.movementRange}"
        );
    }
    private TacticalUnit FindUnitAtCell(Vector2Int coord)
    {
        TacticalUnit[] units = FindObjectsOfType<TacticalUnit>();

        foreach (TacticalUnit unit in units)
        {
            if (unit == null || !unit.IsAlive)
                continue;

            if (WorldToGrid(unit.transform.position) == coord)
                return unit;
        }

        return null;
    }
    private void DeselectUnit()
    {
        isSelected = false;
        isJumpMode = false;
        isAttackMode = false;
        isPickupMode = false;
        ClearMovementTiles();
    }
    private void ClearMovementTiles()
    {
        foreach (GameObject tile in movementTiles)
            if (tile != null) Destroy(tile);
        movementTiles.Clear();
        reachableCells.Clear();
        jumpCells.Clear();
        throwCells.Clear();
        attackCells.Clear();
        attackTargets.Clear();
        pickupCells.Clear();
        pickupTargets.Clear();
        throwEnemyTargets.Clear();
        throwFriendlyTargets.Clear();
    }
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - origin.x) / cellSize + (gridWidth - 1) / 2f);
        int z = Mathf.RoundToInt((worldPosition.z - origin.z) / cellSize + (gridDepth - 1) / 2f);
        return new Vector2Int(x, z);
    }
    private Vector3 GridToWorld(Vector2Int coord)
    {
        float x = origin.x + (coord.x - (gridWidth - 1) / 2f) * cellSize;
        float z = origin.z + (coord.y - (gridDepth - 1) / 2f) * cellSize;
        return new Vector3(x, 0f, z);
    }
    private Vector3 CellWorldPosition(Cell cell)
    {
        Vector3 target = GridToWorld(cell.coord);
        Collider playerCollider = actingUnit.GetComponentInChildren<Collider>();
        if (playerCollider != null)
        {
            Vector3 colliderCenterOffset = playerCollider.bounds.center - actingUnit.position;
            target.x -= colliderCenterOffset.x;
            target.z -= colliderCenterOffset.z;
            float pivotToBottom = actingUnit.position.y - playerCollider.bounds.min.y;
            target.y = cell.surfaceY + pivotToBottom;
        }
        else target.y = cell.surfaceY;
        return target;
    }
    private void BuildGrid()
    {
        cells.Clear();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector2Int coord = new Vector2Int(x, z);
                Vector3 worldPosition = GridToWorld(coord);
                Vector3 rayOrigin = new Vector3(worldPosition.x, raycastStartY, worldPosition.z);
                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, 1000f, terrainMask, QueryTriggerInteraction.Ignore);
                System.Array.Sort(hits, (a, b) => b.point.y.CompareTo(a.point.y));
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.layer != walkableLayer || hit.normal.y < 0.5f) continue;
                    cells[coord] = new Cell(coord, hit.point.y);
                    break;
                }
            }
        }
        Debug.Log($"Grid gotowy. Walkable cells: {cells.Count}");
    }

    private List<Cell> FindWalkPath(Vector2Int start, Vector2Int target)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> previous = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();
        queue.Enqueue(start);
        distance[start] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == target) break;
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (!cells.ContainsKey(next) || distance.ContainsKey(next) || IsCellBlocked(next)) continue;
                if (Mathf.Abs(cells[next].surfaceY - cells[current].surfaceY) > maxStepHeight) continue;
                int nextDistance = distance[current] + 1;
                if (nextDistance > (CurrentUnit != null && CurrentUnit.isPlayerControlled ? CurrentUnit.RemainingMovement : movementRange)) continue;
                distance[next] = nextDistance;
                previous[next] = current;
                queue.Enqueue(next);
            }
        }
        if (!distance.ContainsKey(target)) return null;
        List<Cell> path = new List<Cell>();
        Vector2Int step = target;
        while (step != start)
        {
            path.Add(cells[step]);
            if (!previous.TryGetValue(step, out step)) return null;
        }
        path.Reverse();
        return path;
    }

    private IEnumerator FollowPath(List<Cell> path)
    {
        isMoving = true;
        TacticalUnitSprite walkingSprite = actingUnit != null
            ? actingUnit.GetComponent<TacticalUnitSprite>() : null;
        if (walkingSprite != null) walkingSprite.SetWalking(true);
        foreach (Cell cell in path)
        {
            Vector3 startPosition = actingUnit.position;
            Vector3 targetPosition = CellWorldPosition(cell);
            // Face the horizontal direction of THIS step, before moving.
            // This works for player and AI, including turns along a path.
            Vector3 stepDirection = targetPosition - startPosition;
            stepDirection.y = 0f;
            if (stepDirection.sqrMagnitude > 0.0001f)
            {
                TacticalUnitSprite unitSprite = actingUnit.GetComponent<TacticalUnitSprite>();
                if (unitSprite != null) unitSprite.FaceDirection(stepDirection);
            }
            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = Mathf.Max(distance / moveSpeed, 0.01f);
            float elapsed = 0f;
            float heightDifference = Mathf.Abs(targetPosition.y - startPosition.y);
            float arc = heightDifference > 0.01f ? heightDifference * 0.5f : 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);
                position.y += arc * Mathf.Sin(t * Mathf.PI);
                actingUnit.position = position;
                yield return null;
            }
            actingUnit.position = targetPosition;
        }
        if (walkingSprite != null) walkingSprite.SetWalking(false);
        isMoving = false;
        if (CurrentUnit != null) CurrentUnit.SpendMovement(path.Count);
        ClearMovementTiles();
        isSelected = CurrentUnit != null && CurrentUnit.isPlayerControlled;
        isJumpMode = false;
        if (isSelected) RefreshMovementTiles();
    }

    private IEnumerator JumpToCell(Cell targetCell)
    {
        FaceActingUnit(CellWorldPosition(targetCell));
        isMoving = true;
        Vector3 startPosition = actingUnit.position;
        Vector3 targetPosition = CellWorldPosition(targetCell);
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = Mathf.Max(distance / moveSpeed, 0.15f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);
            position.y += Mathf.Sin(t * Mathf.PI) * jumpArcHeight;
            actingUnit.position = position;
            yield return null;
        }
        actingUnit.position = targetPosition;
        isMoving = false;
        if (CurrentUnit != null) CurrentUnit.SpendMovement(Mathf.Abs(WorldToGrid(startPosition).x - targetCell.coord.x) + Mathf.Abs(WorldToGrid(startPosition).y - targetCell.coord.y));
        ClearMovementTiles();
        isSelected = CurrentUnit != null && CurrentUnit.isPlayerControlled;
        isJumpMode = false;
        if (isSelected) RefreshMovementTiles();
    }


    // AI: szukanie trasy po chodzeniu i skokach przez przerwy w platformach.
    private class AIPlanStep
    {
        public Vector2Int coord;
        public bool jump;
        public int cost;
        public AIPlanStep(Vector2Int coord, bool jump, int cost)
        { this.coord = coord; this.jump = jump; this.cost = cost; }
    }

    private List<AIPlanStep> FindAIPlan(Vector2Int start, Vector2Int goal, int budget, int range)
    {
        if (!cells.ContainsKey(start)) return null;
        var distances = new Dictionary<Vector2Int, int> { [start] = 0 };
        var previous = new Dictionary<Vector2Int, Vector2Int>();
        var jumps = new Dictionary<Vector2Int, bool>();
        var costs = new Dictionary<Vector2Int, int>();
        var frontier = new List<Vector2Int> { start };
        while (frontier.Count > 0)
        {
            int bestIndex = 0;
            for (int i = 1; i < frontier.Count; i++)
                if (distances[frontier[i]] < distances[frontier[bestIndex]]) bestIndex = i;
            Vector2Int current = frontier[bestIndex];
            frontier.RemoveAt(bestIndex);
            int currentCost = distances[current];
            foreach (Vector2Int dir in directions)
            {
                for (int distance = 1; distance <= maxJumpGapCells + 1; distance++)
                {
                    Vector2Int next = current + dir * distance;
                    if (!cells.ContainsKey(next) || IsCellBlocked(next)) continue;
                    bool jump = distance > 1;
                    if (jump)
                    {
                        bool validGap = true;
                        for (int k = 1; k < distance; k++)
                        {
                            Vector2Int mid = current + dir * k;
                            if (cells.ContainsKey(mid) || IsBlockingVolumeAt(mid)) { validGap = false; break; }
                        }
                        if (!validGap || Mathf.Abs(cells[next].surfaceY - cells[current].surfaceY) > maxJumpHeight) continue;
                    }
                    else if (Mathf.Abs(cells[next].surfaceY - cells[current].surfaceY) > maxStepHeight) continue;
                    int newCost = currentCost + distance;
                    if (newCost > budget || (distances.ContainsKey(next) && distances[next] <= newCost)) continue;
                    distances[next] = newCost;
                    previous[next] = current;
                    jumps[next] = jump;
                    costs[next] = distance;
                    if (!frontier.Contains(next)) frontier.Add(next);
                }
            }
        }
        Vector2Int destination = start;
        int bestScore = int.MaxValue;
        foreach (var entry in distances)
        {
            Vector2Int cell = entry.Key;
            int dist = Mathf.Abs(cell.x - goal.x) + Mathf.Abs(cell.y - goal.y);
            int score = Mathf.Max(0, dist - range) * 100 + entry.Value;
            if (score < bestScore) { bestScore = score; destination = cell; }
        }
        if (destination == start) return null;
        var plan = new List<AIPlanStep>();
        Vector2Int cursor = destination;
        while (cursor != start)
        {
            plan.Add(new AIPlanStep(cursor, jumps[cursor], costs[cursor]));
            cursor = previous[cursor];
        }
        plan.Reverse();
        return plan;
    }

    // Rzut tylko wtedy, gdy AI ma obok siebie obiekt/jednostkę, którą może unieść,
    // a gracz jest w zasięgu rzutu. Wykorzystuje istniejącą animację rzucania.
    private IEnumerator EnemyTryThrow(TacticalUnit enemy, TacticalUnit target)
    {
        if (enemy == null || target == null || enemy.hasActed || isCarrying) yield break;
        Vector2Int originCell = WorldToGrid(enemy.transform.position);
        Vector2Int targetCell = WorldToGrid(target.transform.position);
        int dist = Mathf.Abs(originCell.x - targetCell.x) + Mathf.Abs(originCell.y - targetCell.y);
        if (dist < 1 || dist > throwRange || !IsStraightThrow(originCell, targetCell)) yield break;

        // Cel i rzucający muszą znajdować się na jednej osi gridu.
        // Wybieramy pole lądowania dokładnie przed celem, bez odchylenia w bok.
        if (!TryGetThrowLandingCell(originCell, targetCell, out Cell landing)) yield break;

        Transform objectToThrow = null;
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = originCell + dir;
            if (!cells.ContainsKey(neighbor)) continue;
            Collider[] hits = Physics.OverlapBox(
                new Vector3(GridToWorld(neighbor).x, cells[neighbor].surfaceY + blockingCheckHeight, GridToWorld(neighbor).z),
                new Vector3(cellSize * 0.4f, blockingCheckHeight + 0.5f, cellSize * 0.4f),
                Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
            foreach (Collider hit in hits)
            {
                Transform candidate = FindCarryableRoot(hit.transform);
                if (candidate == null || candidate == enemy.transform || candidate == target.transform) continue;
                TacticalUnit unit = candidate.GetComponent<TacticalUnit>();
                if (unit != null && (!unit.IsAlive || unit.IsActiveUnit || unit.isPlayerControlled)) continue;
                if (!CanCarry(candidate)) continue;
                objectToThrow = candidate;
                break;
            }
            if (objectToThrow != null) break;
        }
        if (objectToThrow == null) yield break;
        PickUpObject(objectToThrow);
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(ThrowAtEnemy(target.transform, landing));
        enemy.hasActed = true;
    }

    public IEnumerator RunEnemyTurn(TacticalUnit enemy)
    {
        if (enemy == null || !enemy.IsAlive) yield break;
        actingUnit = enemy.transform;
        movementRange = enemy.movementRange;
        moveSpeed = enemy.moveSpeed;
        carryStrength = enemy.strength;
        attackDamage = enemy.attackDamage;
        isArcher = enemy.isArcher;
        archerAttackRange = enemy.attackRange;
        isSelected = false;
        yield return new WaitForSeconds(0.3f);

        TacticalUnit target = null;
        int nearest = int.MaxValue;
        Vector2Int start = WorldToGrid(enemy.transform.position);
        foreach (TacticalUnit candidate in FindObjectsOfType<TacticalUnit>())
        {
            if (candidate == null || !candidate.IsAlive || !candidate.isPlayerControlled) continue;
            Vector2Int pos = WorldToGrid(candidate.transform.position);
            int d = Mathf.Abs(start.x - pos.x) + Mathf.Abs(start.y - pos.y);
            if (d < nearest) { nearest = d; target = candidate; }
        }
        if (target == null) { actingUnit = null; yield break; }

        int range = enemy.isArcher ? enemy.attackRange : 1;
        if (nearest > range)
        {
            List<AIPlanStep> plan = FindAIPlan(start, WorldToGrid(target.transform.position), enemy.RemainingMovement, range);
            if (plan != null)
            {
                foreach (AIPlanStep step in plan)
                {
                    if (!enemy.IsAlive || !target.IsAlive || enemy.RemainingMovement < step.cost) break;
                    if (IsCellBlocked(step.coord)) break;
                    if (step.jump)
                        yield return StartCoroutine(JumpToCell(cells[step.coord]));
                    else
                        yield return StartCoroutine(FollowPath(new List<Cell> { cells[step.coord] }));
                    Vector2Int now = WorldToGrid(enemy.transform.position);
                    Vector2Int goal = WorldToGrid(target.transform.position);
                    if (Mathf.Abs(now.x - goal.x) + Mathf.Abs(now.y - goal.y) <= range) break;
                }
            }
        }
        if (enemy.IsAlive && target.IsAlive && !enemy.hasActed)
        {
            yield return StartCoroutine(EnemyTryThrow(enemy, target));
            if (!enemy.hasActed && IsEnemyInAttackRange(target.transform))
            {
                yield return new WaitForSeconds(0.25f);
                PerformAttack(target.transform);
                enemy.hasActed = true;
            }
        }
        actingUnit = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector2Int coord = new Vector2Int(x, z);
                Vector3 position = GridToWorld(coord);
                if (Application.isPlaying && cells.TryGetValue(coord, out Cell cell)) position.y = cell.surfaceY + 0.02f;
                else position.y += 0.02f;
                Gizmos.DrawWireCube(position, new Vector3(cellSize, 0.02f, cellSize));
            }
        }
    }
}