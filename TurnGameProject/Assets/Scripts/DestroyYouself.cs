using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DestroyYouself : MonoBehaviour
{
    public float timer;
    public float timeBetweenFiring;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenFiring)
        {
            Destroy(this.gameObject);
        }
    }
}