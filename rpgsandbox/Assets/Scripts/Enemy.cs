using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Material flashMat;
    [SerializeField] private float duration;
    public int health;
    SpriteRenderer sr;
    private Material orMat;
    private Coroutine flashCorutine;
    private GameObject player;
    //knockback
    public float knockback;
    public Vector2 startPos;
    Rigidbody2D rb;
    public bool test;
    //
    public float speed;
    public int startPos2;
    public Transform[] points;
    Animator anim;
    private int i;
    void Start()
    {
        if (test == false)
        {
            transform.position = points[startPos2].position;
        }
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        orMat = sr.material;
        flashMat = new Material(flashMat);
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (health <= 0)
        {
            Debug.Log("zdech³");
        }
        if (test == false)
        {
            patrol();
        }
    }
    void Flip()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
    }
    public IEnumerator Flash()
    {
        sr.material = flashMat;
        yield return new WaitForSeconds(duration);
        if (test == true)
        {
            transform.position = startPos;
        }
        sr.material = orMat;
        flashCorutine = null;
    }
    public void Hit()
    {
        if (player.GetComponent<Movement>().facingRight == true)
        {
            rb.velocity = new Vector2(knockback, knockback);
            health--;
        }
        else
        {
            rb.velocity = new Vector2(-knockback, knockback); 
            health--;
        }

        if(flashCorutine != null)
        {
            StopCoroutine(flashCorutine);
        }
        flashCorutine = StartCoroutine(Flash());
    }

    public void patrol()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == 1)
            {
                StartCoroutine(czekaj());
            }
            if (i == 0)
            {
                StartCoroutine(czekaj1());
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        anim.SetBool("walk", true);
    }

    IEnumerator czekaj()
    {
        anim.SetBool("walk", false);
        yield return new WaitForSeconds(4f);
        Flip();
        i = 0;
    }
    IEnumerator czekaj1()
    {
        anim.SetBool("walk", false);
        yield return new WaitForSeconds(4f);
        Flip();
        i = 1;
    }
}
