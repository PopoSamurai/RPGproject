using UnityEngine;
using System.Collections.Generic;
// Dla NavMeshPlus (2D):
using NavMeshPlus.Components;
using UnityEngine.AI;

////////////////////////////////////////////////////////////
// WeightedTile – prefab + waga losowania
////////////////////////////////////////////////////////////
[System.Serializable]
public class WeightedTile
{
    public GameObject prefab; // Prefab kafla
    public float weight = 1f; // Waga (> 0), im wiêksza tym czêœciej wypada
}

////////////////////////////////////////////////////////////
// MapGenerator – generator lochów 5 poziomów
// - TYLKO generowanie proceduralne
// - Bez predefiniowanych map, zapisu, SpawnPoint, itp.
////////////////////////////////////////////////////////////
[DisallowMultipleComponent]
[AddComponentMenu("ProcGen/Map Generator (5 Dungeons)")]
public class MapGenerator : MonoBehaviour
{
    public NavMeshSurface surface;
    public float minRebuildInterval = 0.1f;

    [Header("Parametry mapy")]
    [Min(2)] public int mapWidth = 20;      // Szerokoœæ siatki
    [Min(2)] public int mapHeight = 20;     // Wysokoœæ siatki
    [Min(1)] public int maxCorridors = 60;  // Ile kroków korytarzy maks.
    [Min(0.1f)] public float tileSize = 12.8f; // Rozmiar œwiata na 1 kratkê

    [Header("Losowoœæ (opcjonalne)")]
    public bool useSeed = false;
    public int seed = 0;

    [Header("Sterowanie startem")]
    public bool autoGenerateOnStart = false;
    [Range(1, 5)] public int lochToGenerateOnStart = 1;

    [Header("Prefab gracza (wymagany)")]
    public GameObject playerPrefab;

    [Header("Loch 1 (prefaby + wagi)")]
    public WeightedTile[] tilePrefabsLoch1;
    public GameObject startTilePrefabLoch1;
    public GameObject bossTilePrefabLoch1;

    [Header("Loch 2 (prefaby + wagi)")]
    public WeightedTile[] tilePrefabsLoch2;
    public GameObject startTilePrefabLoch2;
    public GameObject bossTilePrefabLoch2;

    [Header("Loch 3 (prefaby + wagi)")]
    public WeightedTile[] tilePrefabsLoch3;
    public GameObject startTilePrefabLoch3;
    public GameObject bossTilePrefabLoch3;

    [Header("Loch 4 (prefaby + wagi)")]
    public WeightedTile[] tilePrefabsLoch4;
    public GameObject startTilePrefabLoch4;
    public GameObject bossTilePrefabLoch4;

    [Header("Loch 5 (prefaby + wagi)")]
    public WeightedTile[] tilePrefabsLoch5;
    public GameObject startTilePrefabLoch5;
    public GameObject bossTilePrefabLoch5;

    // Bie¿¹cy zestaw (ustalany przez GenerateLoch)
    private WeightedTile[] currentTilePrefabs;
    private GameObject currentStartTilePrefab;
    private GameObject currentBossTilePrefab;

    // Stan generacji
    private GameObject[,] map;                    // siatka kafli
    private readonly List<GameObject> tiles = new List<GameObject>(); // do czyszczenia
    private Transform mapParent;                  // kontener w hierarchii
    private GameObject player;                    // instancja gracza
    private Vector2Int startGrid;                 // pozycja startu na siatce

    // ----------------------------------------
    // Lifecycle
    // ----------------------------------------
    private void Start()
    {
        if (autoGenerateOnStart)
        {
            GenerateLoch(lochToGenerateOnStart);
        }
    }

    private void Update()
    {
        // Szybkie testy: klawisze 1..5 generuj¹ odpowiedni loch
        if (Input.GetKeyDown(KeyCode.Alpha1)) GenerateLoch(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) GenerateLoch(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) GenerateLoch(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) GenerateLoch(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) GenerateLoch(5);
    }

    // ----------------------------------------
    // API publiczne
    // ----------------------------------------

    /// <summary>
    /// Generuje wybrany loch (1..5).
    /// </summary>
    public void GenerateLoch(int index)
    {
        switch (index)
        {
            case 1:
                currentTilePrefabs = tilePrefabsLoch1;
                currentStartTilePrefab = startTilePrefabLoch1;
                currentBossTilePrefab = bossTilePrefabLoch1;
                break;
            case 2:
                currentTilePrefabs = tilePrefabsLoch2;
                currentStartTilePrefab = startTilePrefabLoch2;
                currentBossTilePrefab = bossTilePrefabLoch2;
                break;
            case 3:
                currentTilePrefabs = tilePrefabsLoch3;
                currentStartTilePrefab = startTilePrefabLoch3;
                currentBossTilePrefab = bossTilePrefabLoch3;
                break;
            case 4:
                currentTilePrefabs = tilePrefabsLoch4;
                currentStartTilePrefab = startTilePrefabLoch4;
                currentBossTilePrefab = bossTilePrefabLoch4;
                break;
            case 5:
                currentTilePrefabs = tilePrefabsLoch5;
                currentStartTilePrefab = startTilePrefabLoch5;
                currentBossTilePrefab = bossTilePrefabLoch5;
                break;
            default:
                Debug.LogWarning($"Niepoprawny indeks lochu: {index}. Dozwolone 1–5.");
                return;
        }

        if (!ValidateCurrentSet()) return;

        if (useSeed) Random.InitState(seed);

        GenerateMap();
        // reset siadki NavMesh
        Invoke(nameof(reset_NavMesh), 3f);
    }

    // ----------------------------------------
    // Generacja
    // ----------------------------------------
    private void GenerateMap()
    {
        ClearMap();

        // Kontener w hierarchii dla porz¹dku i ³atwego czyszczenia
        mapParent = new GameObject("MAP_Procedural").transform;
        mapParent.SetParent(transform);

        map = new GameObject[mapWidth, mapHeight];

        // Start w œrodku siatki
        startGrid = new Vector2Int(mapWidth / 2, mapHeight / 2);
        CreateTile(startGrid, currentStartTilePrefab, "StartTile");

        // Korytarze losowym "g³êbokim spacerem"
        GenerateCorridors(startGrid);

        // Kafel Bossa – najdalszy od startu (manhattan)
        PlaceBossTileFarthestFromStart();

        // Gracz na starcie
        SpawnPlayerAtStart();
        
        
    }

    private void GenerateCorridors(Vector2Int startPos)
    {
        int corridors = 0;
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        Vector2Int currentPos = startPos;
        path.Push(currentPos);

        while (corridors < maxCorridors && path.Count > 0)
        {
            // Cztery kierunki 4-neighbour (N,E,S,W)
            Vector2Int[] cardinal =
            {
                new Vector2Int(1, 0),   // prawo
                new Vector2Int(-1, 0),  // lewo
                new Vector2Int(0, 1),   // góra
                new Vector2Int(0, -1)   // dó³
            };

            // Zbierz dostêpne kierunki
            List<Vector2Int> valid = new List<Vector2Int>(4);
            foreach (var dir in cardinal)
            {
                Vector2Int np = currentPos + dir;
                if (IsInside(np) && map[np.x, np.y] == null)
                    valid.Add(dir);
            }

            if (valid.Count > 0)
            {
                // IdŸ w wybranym kierunku
                Vector2Int chosen = valid[Random.Range(0, valid.Count)];
                currentPos += chosen;

                // Wylosuj prefab kafla po wagach i postaw
                GameObject tilePrefab = GetRandomWeightedTile(currentTilePrefabs);
                CreateTile(currentPos, tilePrefab, "Tile");

                path.Push(currentPos);
                corridors++;
            }
            else
            {
                // Cofnij siê o jeden krok (do poprzedniego wêz³a)
                path.Pop();
                if (path.Count > 0)
                    currentPos = path.Peek();
            }
        }
    }

    // ----------------------------------------
    // Narzêdzia generacji
    // ----------------------------------------

    private bool IsInside(Vector2Int p)
    {
        return p.x >= 0 && p.x < mapWidth && p.y >= 0 && p.y < mapHeight;
    }

    private void CreateTile(Vector2Int gridPos, GameObject prefab, string name)
    {
        if (prefab == null) return;

        Vector3 worldPos = new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0f);
        GameObject go = Instantiate(prefab, worldPos, Quaternion.identity, mapParent);
        go.name = name;
        map[gridPos.x, gridPos.y] = go;
        tiles.Add(go);
    }

    private void ReplaceTile(Vector2Int gridPos, GameObject newPrefab, string name)
    {
        GameObject old = map[gridPos.x, gridPos.y];
        if (old != null)
        {
            tiles.Remove(old);
            Destroy(old);
        }
        CreateTile(gridPos, newPrefab, name);
    }

    private GameObject GetRandomWeightedTile(WeightedTile[] weightedTiles)
    {
        // Zbierz tylko poprawne wpisy
        List<WeightedTile> valid = new List<WeightedTile>();
        float total = 0f;

        foreach (var wt in weightedTiles)
        {
            if (wt != null && wt.prefab != null && wt.weight > 0f)
            {
                valid.Add(wt);
                total += wt.weight;
            }
        }

        if (valid.Count == 0)
        {
            Debug.LogError("Brak poprawnych WeightedTile w aktualnym lochu!");
            return null;
        }

        float r = Random.value * total;
        float cum = 0f;
        for (int i = 0; i < valid.Count; i++)
        {
            cum += valid[i].weight;
            if (r <= cum)
                return valid[i].prefab;
        }

        // Bezpiecznik
        return valid[valid.Count - 1].prefab;
    }

    private void PlaceBossTileFarthestFromStart()
    {
        if (currentBossTilePrefab == null)
        {
            Debug.LogWarning("Brak bossTilePrefab w aktualnym lochu – pomijam ustawienie bossa.");
            return;
        }

        int bestDist = -1;
        Vector2Int bestPos = startGrid;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (map[x, y] == null) continue;
                if (x == startGrid.x && y == startGrid.y) continue;

                int dist = Mathf.Abs(x - startGrid.x) + Mathf.Abs(y - startGrid.y);
                if (dist > bestDist)
                {
                    bestDist = dist;
                    bestPos = new Vector2Int(x, y);
                }
            }
        }

        if (bestDist >= 0)
        {
            ReplaceTile(bestPos, currentBossTilePrefab, "BossTile");
        }
        else
        {
            Debug.LogWarning("Nie znaleziono miejsca na kafel bossa (zbyt ma³o wygenerowanych kafli?).");
        }
    }

    private void SpawnPlayerAtStart()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("Brak Player Prefab – pomijam spawn gracza.");
            return;
        }

        if (player != null) Destroy(player);

        Vector3 worldPos = new Vector3(startGrid.x * tileSize, startGrid.y * tileSize, 0f);
        player = Instantiate(playerPrefab, worldPos, Quaternion.identity);
        player.name = "Player";
    }

    // ----------------------------------------
    // Czyszczenie
    // ----------------------------------------
    public void ClearMap()
    {
        // Usuñ kafle
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] != null)
                Destroy(tiles[i]);
        }
        tiles.Clear();

        // Usuñ kontener
        if (mapParent != null)
        {
            Destroy(mapParent.gameObject);
            mapParent = null;
        }

        // Wyzeruj siatkê
        map = null;

        // Usuñ gracza (tylko przy nowej generacji)
        if (player != null)
        {
            Destroy(player);
            player = null;
        }
    }
    public void reset_NavMesh()
    {

        surface.BuildNavMeshAsync();
    }
    // ----------------------------------------
    // Walidacja i pomoc
    // ----------------------------------------
    private bool ValidateCurrentSet()
    {
        if (currentStartTilePrefab == null)
        {
            Debug.LogError("Brak StartTile Prefab w wybranym lochu!");
            return false;
        }
        if (currentTilePrefabs == null || currentTilePrefabs.Length == 0)
        {
            Debug.LogError("Brak kafli (WeightedTile[]) w wybranym lochu!");
            return false;
        }
        return true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (mapWidth < 2) mapWidth = 2;
        if (mapHeight < 2) mapHeight = 2;
        if (maxCorridors < 1) maxCorridors = 1;
        if (tileSize <= 0f) tileSize = 0.1f;
    }
#endif
}

