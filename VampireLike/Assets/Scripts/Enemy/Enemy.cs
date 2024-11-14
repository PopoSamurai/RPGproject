using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;
    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<Movement>().transform;
    }
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.curretMoveSpped * Time.deltaTime);
    }
}