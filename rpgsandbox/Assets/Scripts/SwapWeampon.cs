using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeWeampon
{
    sword,
    bow,
    staff
}
public class SwapWeampon : MonoBehaviour
{
    public int idNumber;
    public string weamponName;
    public int damage;
    public TypeWeampon type;
    public Sprite icon;
    SpriteRenderer sr;
    private GameObject player;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = icon;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player.GetComponent<Weampon>().id = idNumber;
            player.GetComponent<Weampon>().weamponSprite.sprite = icon;
            Destroy(this.gameObject);
        }
    }
}
