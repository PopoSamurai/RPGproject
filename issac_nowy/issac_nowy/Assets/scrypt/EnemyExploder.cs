// EnemyExploder.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyExploder : MonoBehaviour
{
    [Header("Ruch")]
    public float speed = 5f;

    [Header("Wybuch")]
    public float explodeDistance = 1.2f;
    public float explosionRadius = 2.2f;
    public float explosionDamage = 40f;
    public ProjectileTarget damageTarget = ProjectileTarget.Player; // kogo rani wybuch
    public LayerMask obstacleMask; // opcjonalnie, by nie �widzie� przez �ciany

    [Header("FX (opcjonalnie)")]
    public GameObject explosionVfx;

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

        // Bieg do gracza
        Vector2 dir = toPlayer.normalized;
        rb.linearVelocity = dir * speed;

        // Detonacja w pobli�u
        if (dist <= explodeDistance)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionVfx) Instantiate(explosionVfx, transform.position, Quaternion.identity);

        // Zadaj obra�enia w promieniu
        var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var h in hits)
        {
            if (damageTarget == ProjectileTarget.Player && h.CompareTag("Player"))
                ApplyDamage(h.gameObject);
            else if (damageTarget == ProjectileTarget.Enemy && h.CompareTag("Enemy") && h.gameObject != gameObject)
                ApplyDamage(h.gameObject);
        }

        Destroy(gameObject);
    }

    private void ApplyDamage(GameObject target)
    {
        if (target.TryGetComponent<IDamageable>(out var dmg))
            dmg.TakeDamage(explosionDamage);
        else if (target.TryGetComponent<Health>(out var hp))
            hp.TakeDamage(explosionDamage);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}
