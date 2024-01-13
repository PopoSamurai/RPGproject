using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeongo
{
    public class Movement : MonoBehaviour
    {
        public float speed;
        Rigidbody2D rb;
        public Vector3 change;
        Animator anim;
        public GameObject point;
        public ParticleSystem dust;
        public Material defaultM;
        public Material hitMaterial;
        SpriteRenderer sr;
        public float fix;
        public bool isDashing = false;
        public float dashPower;
        public float dashingTime;
        public bool canDash = true;
        public Stamina sta;
        public bool playerIsRange = false;
        AmmoChest ammoBox;
        public GameObject health;
        public bool move = true;
        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            dust.Stop();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            point = GameObject.FindGameObjectWithTag("Aim");
            health = GameObject.FindGameObjectWithTag("HpBar");
            if (move == true)
            {
                Vector3 targ = point.transform.position;
                targ.z = 0f;

                Vector3 objectPos = transform.position;
                targ.x = targ.x - objectPos.x;
                targ.y = targ.y - objectPos.y;

                change = Vector3.zero;
                change.x = Input.GetAxisRaw("Horizontal");
                change.y = Input.GetAxisRaw("Vertical");
                anim.SetFloat("floatX", targ.x);
                anim.SetFloat("floatY", targ.y);
                updateAnimationAndMove();

                if (Input.GetKeyDown(KeyCode.Space) && canDash == true && sta.staminaCost > 0)
                {
                    Physics.IgnoreLayerCollision(8, 6, true);
                    Physics2D.IgnoreLayerCollision(8, 6, true);
                    StartCoroutine(Dash());
                }
            }
        }
        public IEnumerator czekajNa()
        {
            yield return new WaitForSeconds(1f);
            ammoBox.GetComponent<AmmoChest>().stateNumber = 2;
            ammoBox.GetComponent<AmmoChest>().active = false;
        }
        private void FixedUpdate()
        {
            if(isDashing)
            {
                return;
            }
        }

        void updateAnimationAndMove()
        {
            Vector3 targ = point.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            if (change != Vector3.zero)
            {
                Dust();
                MoveCharacter();
                anim.SetFloat("floatX", targ.x);
                anim.SetFloat("floatY", targ.y);
                anim.SetBool("move", true);
            }
            else
            {
                anim.SetBool("move", false);
            }
        }
        //dash
        public IEnumerator Dash()
        {
            sta.staminaCost--;
            canDash = false;
            anim.SetBool("Dash", true);
            isDashing = true;
            rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
            yield return new WaitForSeconds(dashingTime);
            rb.velocity = Vector3.zero;
            Physics.IgnoreLayerCollision(8, 6, false);
            Physics2D.IgnoreLayerCollision(8, 6, false);
            anim.SetBool("Dash", false);
            isDashing = false;
            canDash = true;
        }

        public void MoveCharacter()
        {
            rb.MovePosition(transform.position + change.normalized * speed * Time.fixedDeltaTime);
            //dust.Stop();
        }

        public void Dust()
        {
            dust.Play();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                sr.material = hitMaterial;
                Vector2 difference = transform.position - other.transform.position;
                transform.position = new Vector2(transform.position.x + difference.x * fix, transform.position.y + difference.y * fix);
                health.GetComponent<Health>().health -= 1;
                StartCoroutine(czekajnaHita());
            }
            if(other.CompareTag("Chest"))
            {
                playerIsRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Chest"))
            {
                playerIsRange = false;
            }
        }
        public IEnumerator czekajnaHita()
        {
            yield return new WaitForSeconds(0.3f);
            sr.material = defaultM;
        }
    }
}
