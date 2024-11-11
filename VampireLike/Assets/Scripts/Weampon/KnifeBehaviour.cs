using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehaviour : ProjectWeampon
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        transform.position += direction * weamponData.Spped * Time.deltaTime;
    }
}
