using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    [HideInInspector]
    public float curretMoveSpped;
    [HideInInspector]
    public float curretHealth;
    [HideInInspector]
    public float curretDamage;

    public float despawnDistance = 20f;
    Transform player;
    void Awake()
    {
        curretMoveSpped = enemyData.MoveSpeed;
        curretHealth = enemyData.MaxHealth;
        curretDamage = enemyData.Damage;
    }
    private void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
    }
    private void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnwmy();
        }
    }
    public void takeDamage(float dmg)
    {
        curretHealth -= dmg;

        if(curretHealth <= 0)
        {
            Kill();
        }
    }
    void Kill()
    {
        Destroy(gameObject);
    }
    private void OnCollisionStay2D(Collision2D coll)
    {
        if(coll.gameObject.CompareTag("Player"))
        {
            PlayerStats player = coll.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(curretHealth);
        }
    }
    private void OnDestroy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.OnEnemyKilled();
    }
    void ReturnEnwmy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoint[Random.Range(0, es.relativeSpawnPoint.Count)].position;
    }
}