using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TacticalGridMovement : MonoBehaviour
{
    [Header("Grid")]
    public Transform player;
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
    public int movementRange = 5;
    public float maxStepHeight = 1.1f;
    public float moveSpeed = 4f;

    [Header("Jump action")]
    [Min(1)] public int maxJumpGapCells = 1;
    public float maxJumpHeight = 1.5f;
    public float jumpArcHeight = 1.25f;

    [Header("Pickup / Throw")]
    public Transform carryableUnit;
    public int throwRange = 3;
    [Min(1)] public int carryStrength = 5;
    public GameObject pickupTilePrefab;
    public float throwArcHeight = 2f;
    public float carryHeight = 1.5f;

    [Header("Attack")]
    public bool isArcher = false;
    public string enemyTag = "enemy";
    [Min(1)] public int archerAttackRange = 4;
    [Min(1)] public int attackDamage = 1;

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

        player = unit;
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
            Debug.LogError("Nieprawid³owe ustawienia siatki.");
            enabled = false;
            return;
        }

        BuildGrid();
        ClearMovementTiles(); // Na pocz¹tku gry zasiêg jest ukryty.
    }

    private void Update()
    {
        if (isMoving || player == null || gameCamera == null) return;

        if (isSelected && Input.GetKeyDown(KeyCode.F) && !isCarrying)
        {
            isAttackMode = !isAttackMode;
            isJumpMode = false;
            RefreshMovementTiles();
            Debug.Log(isAttackMode ? "Tryb ATAKU: kliknij czerwone pole pod wrogiem." : "Tryb ataku wy³¹czony.");
        }

        if (isSelected && Input.GetKeyDown(KeyCode.J) && !isCarrying && !isAttackMode)
        {
            isJumpMode = !isJumpMode;
            RefreshMovementTiles();
            Debug.Log(isJumpMode ? "Tryb SKOKU: wybierz podœwietlone pole l¹dowania." : "Tryb CHODZENIA.");
        }

        if (isSelected && Input.GetKeyDown(KeyCode.E) && !isCarrying && !isAttackMode)
            EnterPickupMode();

        if (Input.GetMouseButtonDown(0)) HandleClick();
        if (isSelected && Input.GetKeyDown(KeyCode.Escape)) DeselectUnit();
    }

    private void HandleClick()
    {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, terrainMask, QueryTriggerInteraction.Ignore))
            return;
        if (hit.normal.y < 0.5f) return;

        Vector2Int clickedCoord = WorldToGrid(hit.point);
        Vector2Int playerCoord = WorldToGrid(player.position);

        if (!isCarrying &&
            !isAttackMode &&
            !isPickupMode &&
            !isJumpMode)
        {
            TacticalUnit clickedUnit = FindUnitAtCell(clickedCoord);

            if (clickedUnit != null && clickedUnit.isPlayerControlled)
            {
                SelectUnit(clickedUnit);
                return;
            }
        }
        if (clickedCoord == playerCoord)
        {
            SelectUnit(player);
            return;
        }

        if (!isSelected) return;

        if (isPickupMode)
        {
            if (!pickupTargets.TryGetValue(clickedCoord, out Transform selectedObject) || selectedObject == null)
                return;
            if (!CanCarry(selectedObject))
            {
                Debug.Log("Ten obiekt jest zbyt ciê¿ki, by go podnieœæ.");
                return;
            }

            ClearMovementTiles();
            isPickupMode = false;
            PickUpObject(selectedObject);
            return;
        }

        if (isAttackMode)
        {
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
            if (!throwCells.Contains(clickedCoord))
                return;

            // Klikniêto przeciwnika.
            if (throwEnemyTargets.TryGetValue(
                clickedCoord, out Transform enemy) && enemy != null)
            {
                if (!TryGetThrowLandingCell(
                    WorldToGrid(player.position),
                    WorldToGrid(enemy.position),
                    out Cell landingCell))
                {
                    Debug.Log(
                        "Nie mo¿na rzuciæ istot¹. " +
                        "Brak wolnego pola przed przeciwnikiem.");
                    return;
                }

                ClearMovementTiles();
                isSelected = false;

                StartCoroutine(
                    ThrowAtEnemy(enemy, landingCell));

                return;
            }
            // Klikniêto w³asn¹ jednostkê.
            if (throwFriendlyTargets.TryGetValue(
                clickedCoord, out Transform friendly) &&
                friendly != null)
            {
                if (!TryGetThrowLandingCell(
                    WorldToGrid(player.position),
                    WorldToGrid(friendly.position),
                    out Cell landingCell))
                {
                    Debug.Log(
                        "Nie mo¿na rzuciæ jednostk¹. " +
                        "Brak wolnego pola przed celem.");
                    return;
                }

                ClearMovementTiles();
                isSelected = false;

                StartCoroutine(
                    ThrowAtEnemy(friendly, landingCell));

                return;
            }
            // Klikniêto zwyk³e, wolne pole.
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

        if (!reachableCells.Contains(clickedCoord) || IsCellBlocked(clickedCoord)) return;
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
        isMoving = true;

        if (carriedObject == null || enemy == null)
        {
            isMoving = false;
            yield break;
        }

        Transform thrownObject = carriedObject;

        Vector3 startPosition = thrownObject.position;
        Vector3 targetPosition = GridToWorld(landingCell.coord);

        Collider[] carriedColliders =
            thrownObject.GetComponentsInChildren<Collider>();

        targetPosition.y = landingCell.surfaceY + carriedBottomOffset;

        thrownObject.SetParent(null, true);

        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = Mathf.Max(distance / moveSpeed, 0.25f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Jeœli przeciwnik zniknie podczas animacji, nie przerywamy lotu.
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

        foreach (Collider col in carriedColliders)
        {
            if (col != null)
                col.enabled = true;
        }

        Rigidbody carriedBody = thrownObject.GetComponent<Rigidbody>();

        if (carriedBody != null)
            carriedBody.isKinematic = false;

        // Obra¿enia po zakoñczeniu rzutu.
        if (enemy != null)
        {
            enemy.gameObject.SendMessage(
                "ApplyDamage",
                attackDamage,
                SendMessageOptions.DontRequireReceiver);

            Debug.Log(
                $"Rzut istot¹ trafi³ {enemy.name}. " +
                $"Obra¿enia: {attackDamage}.");
        }

        isCarrying = false;
        isMoving = false;
        carriedObject = null;

        ClearMovementTiles();

        isSelected = false;
        isJumpMode = false;
        isAttackMode = false;
        isPickupMode = false;

        Debug.Log("Rzut na przeciwnika zakoñczony.");
    }
    private void RefreshMovementTiles()
    {
        ClearMovementTiles();
        if (player == null || movementTilePrefab == null) return;

        Vector2Int start = WorldToGrid(player.position);
        if (!cells.ContainsKey(start)) return;

        if (isCarrying) BuildThrowTargets(start);
        else if (isPickupMode) BuildPickupTargets(start);
        else if (isAttackMode) BuildAttackTargets(start);
        else if (isJumpMode) BuildJumpTargets(start);
        else BuildWalkTargets(start);

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
        GameObject[] enemies;
        try { enemies = GameObject.FindGameObjectsWithTag(enemyTag); }
        catch (UnityException)
        {
            Debug.LogError($"Nie istnieje tag '{enemyTag}'. Dodaj go w Tags and Layers albo zmieñ Enemy Tag w Inspectorze.");
            return;
        }

        foreach (GameObject enemyObject in enemies)
        {
            if (enemyObject == null || !enemyObject.activeInHierarchy) continue;
            Transform enemy = enemyObject.transform;
            Vector2Int enemyCoord = WorldToGrid(enemy.position);
            int distance = Mathf.Abs(enemyCoord.x - start.x) + Mathf.Abs(enemyCoord.y - start.y);
            int allowedRange = isArcher ? archerAttackRange : 1;
            if (distance < 1 || distance > allowedRange || !cells.ContainsKey(enemyCoord)) continue;
            attackCells.Add(enemyCoord);
            attackTargets[enemyCoord] = enemy;
        }
    }

    private bool IsEnemyInAttackRange(Transform enemy)
    {
        if (enemy == null) return false;
        Vector2Int playerCoord = WorldToGrid(player.position);
        Vector2Int enemyCoord = WorldToGrid(enemy.position);
        int distance = Mathf.Abs(enemyCoord.x - playerCoord.x) + Mathf.Abs(enemyCoord.y - playerCoord.y);
        return distance >= 1 && distance <= (isArcher ? archerAttackRange : 1);
    }

    private void PerformAttack(Transform enemy)
    {
        if (enemy == null) return;
        int damage = attackDamage;
        // Enemy mo¿e mieæ w³asny komponent z metod¹ public void ApplyDamage(int damage).
        enemy.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
        Debug.Log($"{(isArcher ? "Strza³ z ³uku" : "Atak wrêcz")}: {enemy.name}, obra¿enia: {damage}.");
    }

    private void TintTileRed(GameObject tile)
    {
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            // material tworzy instancjê materia³u dla tego konkretnego podœwietlenia.
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
                if (newDistance > movementRange) continue;
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

        // To jest osobna akcja: l¹dowanie musi byæ w linii prostej,
        // a pomiêdzy startem i celem musi istnieæ co najmniej jedno pole bez Walkable.
        foreach (Vector2Int dir in directions)
        {
            for (int gap = 1; gap <= maxJumpGapCells; gap++)
            {
                int landingDistance = gap + 1;
                if (landingDistance > movementRange) break;

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
        // Sprawdzamy wszystkie warstwy, bo przeciwnicy mog¹ nie mieæ osobnej warstwy.
        // Pole blokuje obiekt z Blocking Mask LUB obiekt oznaczony tagiem Enemy.
        Collider[] obstacles = Physics.OverlapBox(
            center,
            new Vector3(cellSize * 0.35f, blockingCheckHeight, cellSize * 0.35f),
            Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);

        foreach (Collider obstacle in obstacles)
        {
            if (obstacle == null) continue;
            // W³asne i wrogie jednostki blokuj¹ zajêt¹ kratkê.
            TacticalUnit occupiedUnit =
                obstacle.GetComponentInParent<TacticalUnit>();

            if (occupiedUnit != null)
            {
                if (player != null &&
                    (occupiedUnit.transform == player ||
                     occupiedUnit.transform.IsChildOf(player)))
                    continue;

                if (isCarrying &&
                    carriedObject != null &&
                    (occupiedUnit.transform == carriedObject ||
                     occupiedUnit.transform.IsChildOf(carriedObject)))
                    continue;

                return true;
            }

            if (player != null && (obstacle.transform == player || obstacle.transform.IsChildOf(player)))
                continue;
            if (isCarrying && carryableUnit != null && (obstacle.transform == carryableUnit || obstacle.transform.IsChildOf(carryableUnit)))
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
            if (player != null && (obstacle.transform == player || obstacle.transform.IsChildOf(player))) continue;
            if (isCarrying && carryableUnit != null && (obstacle.transform == carryableUnit || obstacle.transform.IsChildOf(carryableUnit))) continue;

            bool onBlockingLayer = (blockingMask.value & (1 << obstacle.gameObject.layer)) != 0;
            if (onBlockingLayer || IsEnemyCollider(obstacle)) return true;
        }
        return false;
    }

    private bool IsEnemyCollider(Collider obstacle)
    {
        if (obstacle == null) return false;

        // Tag mo¿e znajdowaæ siê na obiekcie g³ównym, a Collider na obiekcie potomnym.
        Transform current = obstacle.transform;
        while (current != null)
        {
            try
            {
                if (current.CompareTag(enemyTag)) return true;
            }
            catch (UnityException)
            {
                // Jeœli tag nie istnieje, pomijamy sprawdzanie tagu.
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
            Debug.Log("Nie ma obok ¿adnych obiektów, które mo¿esz podnieœæ.");
        else
            Debug.Log("Wybierz podœwietlone pole pod jednostk¹ lub skrzyni¹, któr¹ chcesz podnieœæ.");
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
                if (candidate == null || candidate == player || candidate.IsChildOf(player)) continue;
                if (isCarrying && carriedObject == candidate) continue;
                if (!IsCarryableObject(candidate) || !CanCarry(candidate)) continue;

                pickupTargets[coord] = candidate;
                pickupCells.Add(coord);
                break;
            }
        }

        // Obs³uga obiektu przypisanego w Inspectorze, jeœli znajduje siê obok.
        if (carryableUnit != null)
        {
            Vector2Int coord = WorldToGrid(carryableUnit.position);
            int distance = Mathf.Abs(coord.x - start.x) + Mathf.Abs(coord.y - start.y);
            if (distance == 1 && cells.ContainsKey(coord) && CanCarry(carryableUnit))
            {
                pickupTargets[coord] = carryableUnit;
                pickupCells.Add(coord);
            }
        }
    }

    private Transform FindCarryableRoot(Transform candidate)
    {
        Transform current = candidate;
        while (current != null && current != player)
        {
            if (IsCarryableObject(current)) return current;
            current = current.parent;
        }
        return null;
    }

    private bool IsCarryableObject(Transform candidate)
    {
        if (candidate == null || candidate == player || candidate.IsChildOf(player)) return false;

        // Skrzynki i obiekty na Blocking Mask mo¿na podnosiæ.
        if ((blockingMask.value & (1 << candidate.gameObject.layer)) != 0) return true;

        // Wrogowie mog¹ byæ podnoszeni po tagu enemy, tak¿e gdy tag jest na rodzicu.
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

        // Zgodnoœæ z wczeœniej przypisanym Carryable Unit.
        return carryableUnit != null && (candidate == carryableUnit || candidate.IsChildOf(carryableUnit));
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

        carriedObject = objectToCarry;
        Collider[] carriedColliders = carriedObject.GetComponentsInChildren<Collider>();
        carriedBottomOffset = 0.5f;
        foreach (Collider col in carriedColliders)
            carriedBottomOffset = Mathf.Max(carriedBottomOffset, carriedObject.position.y - col.bounds.min.y);

        isCarrying = true;
        carriedObject.SetParent(player, true);
        carriedObject.position = player.position + Vector3.up * carryHeight;
        foreach (Collider col in carriedColliders) col.enabled = false;

        Rigidbody carriedBody = carriedObject.GetComponent<Rigidbody>();
        if (carriedBody != null) carriedBody.isKinematic = true;

        isJumpMode = false;
        isPickupMode = false;
        RefreshMovementTiles();
        Debug.Log($"Podniesiono: {carriedObject.name}. Kliknij podœwietlone pole, aby rzuciæ.");
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

                if (distance <= throwRange)
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

            if (distance < 1 || distance > throwRange)
                continue;

            throwCells.Add(enemyCoord);
            throwEnemyTargets[enemyCoord] = enemy;
        }
        // Wyszukiwanie pozosta³ych jednostek, w które mo¿na rzuciæ.
        Collider[] allColliders = Physics.OverlapBox(
            player.position,
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

            if (candidate == player || candidate.IsChildOf(player))
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

            if (distance < 1 || distance > throwRange)
                continue;

            // Przeciwnicy s¹ obs³ugiwani przez osobny s³ownik.
            if (throwEnemyTargets.ContainsKey(coord))
                continue;

            throwCells.Add(coord);
            throwFriendlyTargets[coord] = candidate;
        }
    }
    private IEnumerator ThrowUnitToCell(Cell targetCell)
    {
        isMoving = true;
        if (carriedObject == null)
        {
            isCarrying = false;
            isMoving = false;
            yield break;
        }

        Vector3 startPosition = carriedObject.position;
        Vector3 targetPosition = GridToWorld(targetCell.coord);
        Collider[] carriedColliders = carriedObject.GetComponentsInChildren<Collider>();
        targetPosition.y = targetCell.surfaceY + carriedBottomOffset;

        carriedObject.SetParent(null, true);
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
        foreach (Collider col in carriedColliders) col.enabled = true;
        Rigidbody carriedBody = carriedObject.GetComponent<Rigidbody>();
        if (carriedBody != null) carriedBody.isKinematic = false;

        isCarrying = false;
        isMoving = false;
        carriedObject = null;
        ClearMovementTiles();
        isSelected = false;
        isJumpMode = false;
        Debug.Log("Rzut zakoñczony.");
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
        if (unit == null || !unit.IsAlive || !unit.isPlayerControlled)
            return;

        if (isCarrying || isMoving)
            return;

        player = unit.transform;

        // Statystyki kontrolera pochodz¹ od zaznaczonej jednostki.
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
            $"Si³a: {unit.strength} | " +
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
        Collider playerCollider = player.GetComponentInChildren<Collider>();
        if (playerCollider != null)
        {
            Vector3 colliderCenterOffset = playerCollider.bounds.center - player.position;
            target.x -= colliderCenterOffset.x;
            target.z -= colliderCenterOffset.z;
            float pivotToBottom = player.position.y - playerCollider.bounds.min.y;
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
                if (nextDistance > movementRange) continue;
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
        foreach (Cell cell in path)
        {
            Vector3 startPosition = player.position;
            Vector3 targetPosition = CellWorldPosition(cell);
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
                player.position = position;
                yield return null;
            }
            player.position = targetPosition;
        }
        isMoving = false;
        ClearMovementTiles();
        isSelected = false;
        isJumpMode = false;
    }

    private IEnumerator JumpToCell(Cell targetCell)
    {
        isMoving = true;
        Vector3 startPosition = player.position;
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
            player.position = position;
            yield return null;
        }
        player.position = targetPosition;
        isMoving = false;
        ClearMovementTiles();
        isSelected = false;
        isJumpMode = false;
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