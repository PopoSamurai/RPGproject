// EnemySideShooter.cs (v2 – odbijanie opcjonalne)
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemySideShooter : MonoBehaviour
{
    [Header("Ruch")]
    public float speed = 4f;

    [Header("Odbijanie (opcjonalne)")]
    public bool enableBounce = true;          // ← włącz/wyłącz odbijanie
    public LayerMask bounceMask = ~0;         // warstwy do odbijania (np. Walls)
    public float bounceCheckDistance = 0.6f;  // zasięg czujnika „przed nosem”
    public float bounceCooldown = 0.05f;      // anty-drganie na ścianie

    [Header("Strzelanie bokami")]
    public Projectile sideBulletPrefab; // użyj prefabu z target = Player
    public float fireInterval = 0.8f;
    public float sideOffset = 0.4f;

    private Rigidbody2D rb;
    private float fireTimer;
    private float bounceTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir.sqrMagnitude < 0.1f) dir = Vector2.right;
        rb.linearVelocity = dir * speed;
    }

    private void FixedUpdate()
    {
        // Strzelanie cykliczne
        fireTimer -= Time.fixedDeltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = fireInterval;
            FireSideShots();
        }

        // Opcjonalne odbijanie
        if (enableBounce)
            BounceIfWallAhead();

        // utrzymuj stałą prędkość
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    private void BounceIfWallAhead()
    {
        bounceTimer -= Time.fixedDeltaTime;
        if (bounceTimer > 0f) return;

        Vector2 dir = rb.linearVelocity.normalized;
        if (dir.sqrMagnitude < 0.01f) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, bounceCheckDistance, bounceMask);
        if (hit.collider != null)
        {
            Vector2 reflect = Vector2.Reflect(dir, hit.normal);
            if (reflect.sqrMagnitude < 0.01f) reflect = -dir; // awaryjnie odbij wstecz
            rb.linearVelocity = reflect.normalized * speed;
            bounceTimer = bounceCooldown;
        }
    }

    private void FireSideShots()
    {
        if (!sideBulletPrefab) return;
        Vector2 vel = rb.linearVelocity;
        if (vel.sqrMagnitude < 0.01f) return;

        Vector2 forward = vel.normalized;
        Vector2 left = new Vector2(-forward.y, forward.x);
        Vector2 right = -left;

        Vector3 leftPos = transform.position + (Vector3)(left * sideOffset);
        Vector3 rightPos = transform.position + (Vector3)(right * sideOffset);

        var bL = Instantiate(sideBulletPrefab, leftPos, Quaternion.identity);
        bL.SetOwner(gameObject);
        bL.Launch(left);

        var bR = Instantiate(sideBulletPrefab, rightPos, Quaternion.identity);
        bR.SetOwner(gameObject);
        bR.Launch(right);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.green;
        Vector2 dir = rb.linearVelocity.sqrMagnitude > 0.01f ? rb.linearVelocity.normalized : Vector2.right;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)dir * bounceCheckDistance);
    }
#endif
}
