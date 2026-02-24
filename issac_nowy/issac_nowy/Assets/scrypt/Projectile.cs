// Projectile.cs
using UnityEngine;

public enum ProjectileTarget { Player, Enemy }

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Parametry")]
    public float speed = 12f;
    public float damage = 10f;
    public float lifeTime = 5f;
    public ProjectileTarget target = ProjectileTarget.Player;

    [Header("Kolizje")]
    public LayerMask obstacleMask; // np. warstwa "Obstacles" (opcjonalnie)

    private Rigidbody2D rb;
    private Collider2D col;
    private GameObject owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Despawn), lifeTime);
    }

    public void SetOwner(GameObject ownerObj)
    {
        owner = ownerObj;
        var ownerCol = ownerObj ? ownerObj.GetComponent<Collider2D>() : null;
        if (ownerCol && col) Physics2D.IgnoreCollision(ownerCol, col);
    }

    public void Launch(Vector2 direction)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        rb.linearVelocity = direction * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;

        // Opcjonalnie: zderzenie z przeszkod� ko�czy pocisk
        if (((1 << other.gameObject.layer) & obstacleMask.value) != 0)
        {
            Despawn();
            return;
        }

        // Trafianie celu w zale�no�ci od typu
        if (target == ProjectileTarget.Player && other.CompareTag("Player"))
        {
            TryDamage(other.gameObject);
            Despawn();
        }
        else if (target == ProjectileTarget.Enemy && other.CompareTag("Enemy"))
        {
            TryDamage(other.gameObject);
            Despawn();
        }
    }

    private void TryDamage(GameObject hit)
    {
        if (hit.TryGetComponent<IDamageable>(out var dmg))
            dmg.TakeDamage(damage);
        else if (hit.TryGetComponent<Health>(out var hp))
            hp.TakeDamage(damage);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
