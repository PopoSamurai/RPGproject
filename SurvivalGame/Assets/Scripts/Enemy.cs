using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health= 3;
    public ParticleSystem bloodEffect;

    void Update()
    {
        //ded
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Instantiate(bloodEffect, transform.position, bloodEffect.transform.rotation);
            bloodEffect.Play();
            health --;
        }
    }
}
