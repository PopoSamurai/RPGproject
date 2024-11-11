using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    float curretMoveSpped;
    float curretHealth;
    float curretDamage;

    void Awake()
    {
        curretMoveSpped = enemyData.MoveSpeed;
        curretHealth = enemyData.MaxHealth;
        curretDamage = enemyData.Damage;
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
}