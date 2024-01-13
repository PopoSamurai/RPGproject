using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeongo
{
    public class WeamponController : MonoBehaviour
    {
        public Vector2 newClickedPos;
        public Animator anim;
        //shoot
        public Transform bulletSpawnPoint;
        public GameObject bullet;
        public float bulletShoot;
        public GameObject aim;

        public bool canFire;
        public float timer;
        public float timeBetweenFiring;
        public GameObject lightP;
        public Animator cursor;
        public Sprite[] offBullet;
        public Ammo amo;
        public GameObject particles;
        public Movement player;
        private void Start()
        {
            particles = GameObject.FindGameObjectWithTag("1");
        }
        private void Update()
        {
            aim = GameObject.FindGameObjectWithTag("Aim");
            Vector3 targ = aim.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (!canFire)
            {
                lightP.SetActive(false);
                timer += Time.deltaTime;
                if (timer > timeBetweenFiring)
                {
                    canFire = true;
                    timer = 0;
                    anim.SetBool("shoot", false);
                    cursor.SetBool("click", false);
                }
            }

            if (Input.GetMouseButton(0) && canFire && amo.shootAmmoNumber > 0 && player.isDashing == false)
            {
                particles.GetComponent<ParticleSystem>().Play();
                amo.shootAmmoNumber--;
                lightP.SetActive(true);
                canFire = false;
                anim.SetBool("shoot", true);
                switch(amo.weamponNumber)
                {
                    case 1:
                        amo.sr.sprite = offBullet[0];
                        break;
                    case 2:
                        amo.sr.sprite = offBullet[1];
                        break;
                    case 3:
                        amo.sr.sprite = offBullet[2];
                        break;
                    case 4:
                        amo.sr.sprite = offBullet[3];
                        break;
                    case 5:
                        amo.sr.sprite = offBullet[4];
                        break;
                    case 6:
                        amo.sr.sprite = offBullet[5];
                        break;
                }
                cursor.SetBool("click", true);
                var bullets = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                bullets.GetComponent<Rigidbody2D>().velocity = bulletSpawnPoint.transform.up * bulletShoot;
            }
        }
    }
}