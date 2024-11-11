using System.Collections.Generic;
using UnityEngine;
public class MapGenerator : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float chunkradius;
    public LayerMask LayerMask;

    private Vector3 noTerrainPosition;
    public GameObject currentChunk;
    private Movement pl;

    private List<GameObject> spawnedChunks = new List<GameObject>();
    private HashSet<Vector3> spawnedChunkPositions = new HashSet<Vector3>();
    private GameObject lastChunk;

    public float maxDist;
    public float optimalizeDir;
    private float cooldown;

    void Start()
    {
        pl = FindObjectOfType<Movement>();
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimalized();
    }

    public void ChunkChecker()
    {
        if (currentChunk == null)
        {
            return;
        }

        // generate chunk
        if (CheckAndSpawnChunk("Right", pl.direction.x > 0 && pl.direction.y == 0)) return;
        if (CheckAndSpawnChunk("Left", pl.direction.x < 0 && pl.direction.y == 0)) return;
        if (CheckAndSpawnChunk("Up", pl.direction.x == 0 && pl.direction.y > 0)) return;
        if (CheckAndSpawnChunk("Down", pl.direction.x == 0 && pl.direction.y < 0)) return;
        if (CheckAndSpawnChunk("Right Up", pl.direction.x > 0 && pl.direction.y > 0)) return;
        if (CheckAndSpawnChunk("Right Down", pl.direction.x > 0 && pl.direction.y < 0)) return;
        if (CheckAndSpawnChunk("Left Up", pl.direction.x < 0 && pl.direction.y > 0)) return;
        if (CheckAndSpawnChunk("Left Down", pl.direction.x < 0 && pl.direction.y < 0)) return;
    }

    private bool CheckAndSpawnChunk(string direction, bool condition)
    {
        if (condition)
        {
            Transform directionTransform = currentChunk.transform.Find(direction);
            if (directionTransform == null) return false;

            Vector3 spawnPosition = directionTransform.position;
            if (!Physics2D.OverlapCircle(spawnPosition, chunkradius, LayerMask) && !spawnedChunkPositions.Contains(spawnPosition))
            {
                noTerrainPosition = spawnPosition;
                SpawnChunk();
                return true;
            }
        }
        return false;
    }

    public void SpawnChunk()
    {
        int rand = Random.Range(0, terrainChunks.Count);
        lastChunk = Instantiate(terrainChunks[rand], noTerrainPosition, Quaternion.identity);
        spawnedChunks.Add(lastChunk);
        spawnedChunkPositions.Add(noTerrainPosition);
    }

    void ChunkOptimalized()
    {
        cooldown -= Time.deltaTime;

        if (cooldown > 0f) return;
        cooldown = optimalizeDir;

        for (int i = spawnedChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = spawnedChunks[i];
            float opDist = Vector3.Distance(player.transform.position, chunk.transform.position);

            if (opDist > maxDist)
            {
                spawnedChunkPositions.Remove(chunk.transform.position);
                chunk.SetActive(false);
                spawnedChunks.RemoveAt(i);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}