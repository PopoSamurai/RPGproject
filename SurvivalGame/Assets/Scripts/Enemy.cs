using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public int health= 3;
    public ParticleSystem bloodEffect;
    GameObject player;
    private float speed = 5;
    public float maxSpeed = 5;
    public bool seePlayer = false;
    private Collider[] hitColliders;
    private RaycastHit Hit;
    public float range;
    public float rangeSelect;
    Rigidbody rb;

    public float rangeKnock = 50f;
    public float knockBackZForce = 100;

    private void Start()
    {
        speed = maxSpeed;
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        //ded
        if (health <= 0)
        { 
            Destroy(gameObject);
        }
        //follow player
        if(!seePlayer)
        {
            hitColliders = Physics.OverlapSphere(transform.position, range);
            foreach(var HitColliders in hitColliders)
            {
                if(HitColliders.tag == "Player")
                {
                    player = HitColliders.gameObject;
                    seePlayer = true;
                }
            }
        }
        else
        {
            if(Physics.Raycast(transform.position, (player.transform.position - transform.position), out Hit, rangeSelect))
            {
                if(Hit.collider.tag != "Player")
                {
                    seePlayer = false;
                }
                else
                {
                    var Heading = player.transform.position - transform.position;
                    var Distance = Heading.magnitude;
                    var Direction = Heading / Distance;

                    Vector3 Move = new Vector3(Direction.x * speed, 0, Direction.z * speed);
                    rb.velocity = Move;
                    transform.forward = Move;
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Instantiate(bloodEffect, transform.position, transform.rotation /*,gameObject.transform*/); //bez parenta bokrew znika z œmirci¹
            health --;
            KnockBack();
        }
    }

    public void KnockBack()
    {
        transform.position -= transform.forward * Time.deltaTime * knockBackZForce;
    }
}
