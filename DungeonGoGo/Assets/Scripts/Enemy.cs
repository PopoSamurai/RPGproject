using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeongo
{
    public class Enemy : MonoBehaviour
    {
        public GameObject player;
        public float speed;
        private float distance;
        public float distaneceBetween;
        public Material defaultM;
        public Material hitMaterial;
        SpriteRenderer sr;
        public float fix;
        Animator anim;
        public Vector3 change;
        Rigidbody2D rb;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
        }
        private void Update()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance < distaneceBetween)
            {
                anim.SetBool("stay", false);
                Vector3 targ = player.transform.position;
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
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            }
            else
            {
                anim.SetBool("stay", true);
            }
        }
        void updateAnimationAndMove()
        {
            Vector3 targ = player.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            anim.SetFloat("floatX", targ.x);
            anim.SetFloat("floatY", targ.y);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bullet"))
            {
                sr.material = hitMaterial;
                Vector2 difference = transform.position - other.transform.position;
                transform.position = new Vector2(transform.position.x + difference.x * fix, transform.position.y + difference.y * fix);
                StartCoroutine(czekajnaHita());
            }
        }
        public IEnumerator czekajnaHita()
        {
            yield return new WaitForSeconds(0.3f);
            sr.material = defaultM;
        }
    }
}
