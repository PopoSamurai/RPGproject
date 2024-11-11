using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointContainer : WeamponController
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnKnife = Instantiate(weamponData.Prefab);
        spawnKnife.transform.position = transform.position;
        spawnKnife.GetComponent<KnifeBehaviour>().DirectionChecker(player.lastmovement);

    }
}
