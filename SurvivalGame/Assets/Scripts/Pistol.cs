using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public Transform shootPos;
    public GameObject bulletPrefab;
    public float shootSpeed = 5;
    public bool canFire;
    public float timer;
    public float timeBetweenFiring;
    void Update()
    {
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            //shoot
            GameObject bullet = Instantiate(bulletPrefab, shootPos.position,shootPos.rotation);
            bullet.GetComponent<Rigidbody>().velocity = shootPos.forward * shootSpeed;
            canFire = false;
        }
    }
}
