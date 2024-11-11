using UnityEngine;

public class GarlicController : WeamponController
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnGarlic = Instantiate(weamponData.Prefab);
        spawnGarlic.transform.position = transform.position;
        spawnGarlic.transform.parent = transform;
    }
}