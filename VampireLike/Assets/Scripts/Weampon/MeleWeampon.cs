using UnityEngine;

public class MeleWeampon : MonoBehaviour
{
    public WeamponScriptableObj weamponData;
    public float destroyAfterSeconds;
    //stat
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCoolDownDur;
    protected int currentPierce;
    private void Awake()
    {
        currentDamage = weamponData.Damage;
        currentSpeed = weamponData.Spped;
        currentCoolDownDur = weamponData.CooldownDuration;
        currentPierce = weamponData.Pierce;
    }
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);   
    }
    protected virtual void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Enemy"))
        {
            EnemyStats enemy = coll.GetComponent<EnemyStats>();
            enemy.takeDamage(currentDamage);
        }
    }
}