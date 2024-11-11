using UnityEngine;

public class ProjectWeampon : MonoBehaviour
{
    public WeamponScriptableObj weamponData;

    protected Vector3 direction;
    public float destroyAfterSeconds;
    //stats
    protected float currentDanage;
    protected float currentSpped;
    protected float currentCoolDownDur;
    protected int currentPierce;

    void Awake()
    {
        currentDanage = weamponData.Damage;
        currentSpped = weamponData.Spped;
        currentCoolDownDur = weamponData.CooldownDuration;
        currentPierce = weamponData.Pierce;
    }
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if(dirx < 0 && diry == 0) //left
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if(dirx == 0 && diry < 0) //down
        {
            scale.y = scale.y * -1;
        }
        else if(dirx == 0 && diry > 0) //up
        {
            scale.x = scale.x * -1;
        }
        else if(dir.x > 0 && dir.y > 0) // right up
        {
            rotation.z = 0f;
        }
        else if (dir.x > 0 && dir.y < 0) // right down
        {
            rotation.z = -90f;
        }
        else if(dir.x < 0 && dir.y > 0)
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = -90f;
        }
        else if(dir.x < 0 && dir.y < 0)
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);
    }

    protected virtual void OnTriggerEnter2D(Collider2D coll)
    {
        //reference TakeDamage()

        if(coll.CompareTag("Enemy"))
        {
            EnemyStats enemy = coll.GetComponent<EnemyStats>();
            enemy.takeDamage(currentDanage);
            ReducePierce();
        }
        else if (coll.CompareTag("Prop"))
        {
            if (coll.gameObject.TryGetComponent(out breakableProps brakable))
            {
                brakable.TakeDamage(currentDanage);
                ReducePierce();
            }
        }
    }
    void ReducePierce()
    {
        currentPierce--;
        if(currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}