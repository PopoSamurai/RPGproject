using System.Collections.Generic;
using UnityEngine;

public class GarlicBehaviour : MeleWeampon
{
   List<GameObject> markEnemy;
    protected override void Start()
    {
        base.Start();
        markEnemy = new List<GameObject>();
    }
    protected override void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Enemy") && !markEnemy.Contains(coll.gameObject))
        {
            EnemyStats enemy = coll.GetComponent<EnemyStats>();
            enemy.takeDamage(currentDamage);

            markEnemy.Add(coll.gameObject);
        }
        else if (coll.CompareTag("Prop"))
        {
            if (coll.gameObject.TryGetComponent(out breakableProps brakable) && !markEnemy.Contains(coll.gameObject))
            {
                brakable.TakeDamage(currentDamage);

                markEnemy.Add(coll.gameObject);
            }
        }
    }
}