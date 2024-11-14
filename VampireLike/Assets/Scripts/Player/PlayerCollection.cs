using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;
    private void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        playerCollector.radius = player.currentMagnet;
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.TryGetComponent(out Collect collect))
        {
            Rigidbody2D rb = coll.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDur = (transform.position - coll.transform.position).normalized;
            rb.AddForce(forceDur * pullSpeed);

            collect.Collected();
        }
    }
}