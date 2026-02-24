using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet2D : MonoBehaviour
{
    [Header("Ruch")]
    public float speed = 12f;                 // jednostki/sek
    public bool useTransformRight = true;     // true: leci w prawo lokalne; false: w g�r� lokaln�

    [Header("�ycie pocisku")]
    public float lifeTime = 3f;               // po tylu sekundach �wybucha� i znika

    [Header("Kolizja")]
    public string tagY = "Enemy";             // ustaw na sw�j tag Y
    public string tagZ = "Wall";              // ustaw na sw�j tag Z
    public GameObject impactPrefab;           // co zespawnowa� przy trafieniu / timeoucie
    public bool destroyOnAnyCollision = false;// je�li true � reaguje na ka�dy tag

    Rigidbody2D rb;
    bool ended = false; // �eby nie odpala� efektu wielokrotnie

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.isKinematic = false; // zwykle dynamic dla wykrywania kolizji
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void OnEnable()
    {
        // Timeout
        Invoke(nameof(EndLife), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(EndLife));
    }

    void FixedUpdate()
    {
        // Ruch: Rigidbody2D je�li jest, inaczej transform
        Vector2 dir = (useTransformRight ? (Vector2)transform.right : (Vector2)transform.up);
        if (rb)
            rb.linearVelocity = dir * speed;
        else
            transform.Translate((Vector3)dir * speed * Time.fixedDeltaTime, Space.World);
    }

    // TRIGGER
    void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other.gameObject);
    }

    // ZWYK�A KOLIZJA
    void OnCollisionEnter2D(Collision2D collision)
    {
        TryHit(collision.gameObject);
    }

    void TryHit(GameObject other)
    {
        if (ended) return;

        if (destroyOnAnyCollision ||
            other.CompareTag(tagY) ||
            other.CompareTag(tagZ))
        {
            EndLife();
        }
    }

    void EndLife()
    {
        if (ended) return;
        ended = true;

        if (impactPrefab)
            Instantiate(impactPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}

