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
    private Rigidbody2D rb;
    public bool test;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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
}
