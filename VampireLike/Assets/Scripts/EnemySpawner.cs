using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota; //max mobów w aktualnej fali
        public float spawnInterval; //czas do nastêpnej fali
        public int spawnCount; //iloœæ ju¿ zespawnionych przeciwników w tej fali
    }
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefab;
    }
    public List<Wave> waves;
    public int currentWaveCount;

    float spawnTimer;
    public int enemysAlive;
    public int maxEnemyAllowed;
    public bool maxEnemiesReached = false;
    public float waveInterval;
    public List<Transform> relativeSpawnPoint;
    Transform player;
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CaclucateWaveQuota();
    }

    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0;
            SpawnEnemies();
        }
    }
    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);
         
        if(currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CaclucateWaveQuota();
        }
    }
    void CaclucateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }
        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }
    void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    if(enemysAlive >= maxEnemyAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoint[Random.Range(0, relativeSpawnPoint.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemysAlive++;
                }
            }
        }
        if(enemysAlive < maxEnemyAllowed)
        {
            maxEnemiesReached = false;
        }
    }
    public void OnEnemyKilled()
    {
        enemysAlive--;
    }
}
