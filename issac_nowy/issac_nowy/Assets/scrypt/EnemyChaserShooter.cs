// EnemyChaserShooter.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaserShooter : MonoBehaviour
{
    [Header("Ruch")]
    public float speed = 3.5f;
    public float detectionRange = 12f;

    [Header("Strzelanie")]
    public float shootRange = 8f;
    public float fireRate = 0.6f;
    public Projectile bulletPrefab; // U�YJ prefabu z target = Player
    public Transform shootPoint;    // opcjonalnie null => z pozycji wroga
    private float fireTimer;

    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void FixedUpdate()
    {
        if (!player) return;

        Vector2 toPlayer = (player.position - transform.position);
        float dist = toPlayer.magnitude;

        // Poza detekcj� � st�j
        if (dist > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Gonienie
        rb.linearVelocity = toPlayer.normalized * speed;

        // Strzelanie, je�li blisko
        fireTimer -= Time.fixedDeltaTime;
        if (dist <= shootRange && fireTimer <= 0f)
        {
            fireTimer = fireRate;
            ShootAt(player.position);
        }
    }

    private void ShootAt(Vector3 targetPos)
    {
        if (!bulletPrefab) return;

        Vector3 origin = shootPoint ? shootPoint.position : transform.position;
        Vector2 dir = (targetPos - origin).normalized;

        var b = Instantiate(bulletPrefab, origin, Quaternion.identity);
        b.SetOwner(gameObject);
        b.Launch(dir);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
#endif
}
